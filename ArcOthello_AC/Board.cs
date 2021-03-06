﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArcOthello_AC
{
    [Serializable]
    public class Board : IPlayable.IPlayable
    {
        #region Properties
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        private ObservableCollection<ObservableCollection<Piece>> pieces;
        #endregion

        #region Indexer
        public ObservableCollection<ObservableCollection<Piece>> Pieces
        {
            get { return pieces; }
        }

        public Piece this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= GridHeight)
                    throw new ArgumentOutOfRangeException("row", row, "Invalid Row Index");
                if (col < 0 || col >= GridWidth)
                    throw new ArgumentOutOfRangeException("col", col, "Invalid Column Index");
                return pieces[col][row];
            }
        }
        #endregion

        #region Constructors and Initializers
        public Board(int width, int height)
        {
            this.GridWidth = width;
            this.GridHeight = height;
            this.Init();
        }

        public Board(Board b)
        {
            this.GridWidth = b.GridWidth;
            this.GridHeight = b.GridHeight;
            Init(b.pieces);
        }
        
        /// <summary>
        /// Initializes a board's ObservableCollections (content) and default pieces.
        /// </summary>
        public void Init()
        {
            pieces = new ObservableCollection<ObservableCollection<Piece>>();
            for (int x = 0; x < GridWidth; x++)
            {
                ObservableCollection<Piece> col = new ObservableCollection<Piece>();
                for (int y = 0; y < GridHeight; y++)
                {
                    Piece p = new Piece(Team.None, x, y);
                    col.Add(p);
                }
                pieces.Add(col);
            }
            pieces[3][2].SetTeam(Team.White);
            pieces[3][3].SetTeam(Team.Black);
            pieces[4][2].SetTeam(Team.Black);
            pieces[4][3].SetTeam(Team.White);
        }

        /// <summary>
        /// Initializes a board's ObservableCollections (content) by copying given ObservableCollections.
        /// </summary>
        /// <param name="piecesToCopy">ObservableCollections to copy</param>
        public void Init(ObservableCollection<ObservableCollection<Piece>> piecesToCopy)
        {
            pieces = new ObservableCollection<ObservableCollection<Piece>>();
            for (int x = 0; x < GridWidth; x++)
            {
                ObservableCollection<Piece> col = new ObservableCollection<Piece>();
                for (int y = 0; y < GridHeight; y++)
                {
                    Piece p = new Piece(piecesToCopy[x][y]);
                    col.Add(p);
                }
                pieces.Add(col);
            }
        }
        #endregion

        #region Board control
        /// <summary>
        /// Poses the piece at given position.
        /// Throws exception if the position is invalid.
        /// </summary>
        /// <param name="row">y position on the board for the new piece</param>
        /// <param name="col">x position on the board for the new piece</param>
        /// <param name="p">piece to pose</param>
        public void SetPiece(int row, int col, Piece p)
        {
            if (row < 0 || row >= GridHeight)
                throw new ArgumentOutOfRangeException("row", row, "Invalid Row Index");
            if (col < 0 || col >= GridWidth)
                throw new ArgumentOutOfRangeException("col", col, "Invalid Column Index");
            pieces[col][row] = p;
        }

        /// <summary>
        /// Tries to play at given position and returns if the move is valid.
        /// </summary>
        /// <param name="row">y position on the board for the new piece</param>
        /// <param name="col">x position on the board for the new piece</param>
        /// <param name="team">team whose turn it is</param>
        /// <returns>True if the move is valid false otherwise</returns>
        public bool PosePiece(int row, int col, Team team)
        {
            if (pieces[col][row].Team == (team == Team.White ? 
                                          Team.WhitePreview : 
                                          Team.BlackPreview))
            {
                pieces[col][row].SetTeam(team);
                GetFlipPieceList(row, col, team).ForEach(p => p.Flip());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Displays given team's preview pieces on the board at valid positions.
        /// </summary>
        /// <param name="team">team whose turn it is</param>
        public void ShowPossibleMove(Team team)
        {
            Team preview = team == Team.Black ? Team.BlackPreview : Team.WhitePreview;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (GetFlipPieceList(y, x, team).Count() != 0 && pieces[x][y].Team == Team.None)
                        pieces[x][y].Team = preview;
                }
            }
        }
        
        /// <summary>
        /// Returns a list of pieces to flip for a piece of a given team posed at a given position.
        /// </summary>
        /// <param name="row">y position on the board</param>
        /// <param name="col">x position on the board</param>
        /// <param name="team">piece's team</param>
        /// <returns>list of pieces to flip</returns>
        private List<Piece> GetFlipPieceList(int row, int col, Team team)
        {
            return GetFlipPieceList(row, col, team, 1, 0)
                .Concat(GetFlipPieceList(row, col, team, -1, 0))
                .Concat(GetFlipPieceList(row, col, team, 1, 1))
                .Concat(GetFlipPieceList(row, col, team, -1, -1))
                .Concat(GetFlipPieceList(row, col, team, -1, 1))
                .Concat(GetFlipPieceList(row, col, team, 1, -1))
                .Concat(GetFlipPieceList(row, col, team, 0, 1))
                .Concat(GetFlipPieceList(row, col, team, 0, -1)).ToList();
        }

        /// <summary>
        /// Returns a list of pieces to flip for a piece of a given team posed at a given position.
        /// Checks only a given direction defined by incX / incY.
        /// </summary>
        /// <param name="row">y position on the board</param>
        /// <param name="col">x position on the board</param>
        /// <param name="team">piece's team</param>
        /// <param name="incX">x propagation direction</param>
        /// <param name="incY">y propagation direction</param>
        /// <returns>list of pieces to flip in the given direction</returns>
        private List<Piece> GetFlipPieceList(int row, int col, Team team, int incX, int incY)
        {
            List<Piece> flipPiece = new List<Piece>();

            Team enemyTeam = team == Team.Black ? Team.White : Team.Black;

            row += incY;
            col += incX;

            while (!IsSlotEmpty(row, col) && pieces[col][row].Team == enemyTeam)
            {
                flipPiece.Add(pieces[col][row]);
                row += incY;
                col += incX;
            }

            if (!IsSlotEmpty(row, col) && pieces[col][row].Team == team)
                return flipPiece;
            return new List<Piece>();
        }
        #endregion

        #region Board status
        /// <summary>
        /// Returns the number of valid moves for the given team.
        /// </summary>
        /// <param name="team">team whose turn it is</param>
        /// <returns></returns>
        public int NumberPossibleMove(Team team)
        {
            int count = 0;
            Team preview = team == Team.Black ? Team.BlackPreview : Team.WhitePreview;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    count += GetFlipPieceList(y, x, team).Count() != 0 && pieces[x][y].Team == Team.None ? 1 : 0;
                }
            }
            return count;
        }

        /// <summary>
        /// Returns true if the slot at given position is empty.
        /// </summary>
        /// <param name="row">y position on the board</param>
        /// <param name="col">x position on the board</param>
        /// <returns>true if the slot is empty false otherwise</returns>
        public bool IsSlotEmpty(int row, int col)
        {
            return (col < 0 || col >= GridWidth ||
                    row < 0 || row >= GridHeight ||
                    pieces[col][row].Team == Team.None ||
                    pieces[col][row].Team == Team.BlackPreview ||
                    pieces[col][row].Team == Team.WhitePreview);
        }

        /// <summary>
        /// Returns true if a piece of the given team can be posed at given position.
        /// </summary>
        /// <param name="row">y position on the board</param>
        /// <param name="col">x position on the board</param>
        /// <param name="team">piece's team</param>
        /// <returns>true if the move is valid false otherwise</returns>
        public bool IsValid(int row, int col, Team team)
        {
            return col >= 0 && col < GridWidth && 
                   row >= 0 && row < GridHeight && 
                   pieces[col][row].Team == (team == Team.White ? Team.WhitePreview : Team.BlackPreview);
        }
        #endregion

        #region Preview controls
        /// <summary>
        /// Removes preview pieces from the board.
        /// </summary>
        public void ClearPreview()
        {
            foreach (ObservableCollection<Piece> row in Pieces)
            {
                foreach (Piece p in row)
                {
                    if (IsPreview(p))
                        p.Team = Team.None;
                }
            }
        }

        /// <summary>
        /// Returns true if the given piece is type of preview.
        /// </summary>
        /// <param name="p">piece to test</param>
        /// <returns>true if the given piece's type is preview false otherwise</returns>
        private bool IsPreview(Piece p)
        {
            return p.Team == Team.BlackPreview || p.Team == Team.WhitePreview;
        }
        #endregion

        #region IPlayable Implementation
        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns>IA's name</returns>
        public string GetName()
        {
            return "Jack";
        }

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 8</param>
        /// <param name="row">value between 0 and 6</param>
        /// <param name="isWhite"></param>
        /// <returns>true or false</returns>
        public bool IsPlayable(int column, int row, bool isWhite)
        {
            return IsValid(row, column, isWhite ? Team.White : Team.Black);
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="row">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool PlayMove(int column, int row, bool isWhite)
        {
            bool canPlay = PosePiece(row, column, isWhite ? Team.White : Team.Black);
            if (canPlay)
            {
                ClearPreview();
                ShowPossibleMove(isWhite ? Team.Black : Team.White);
            }
            return canPlay;
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        /// 
        ///             A B C D E F G H I
        ///         [ ][0 1 2 3 4 5 6 7 8]     (first index)
        ///        1 0
        ///        2 1
        ///        3 2        X
        ///        4 3            X
        ///        5 4
        ///        6 5
        ///        7 6
        ///       
        ///          Column Line
        ///  E.g.:    D3, F4 game notation will map to {3,2} resp. {5,3}
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException(); // TODO
        }

        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 7x9 tiles status</returns>
        public int[,] GetBoard()
        {
            int[,] boardInt = new int[GridHeight, GridWidth];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    boardInt[y, x] = this[y, x] == null ? -1 :
                                     this[y, x].Team == Team.White ? 0 :
                                     1;
                }
            }
            return boardInt;
        }

        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns>white player's score</returns>
        public int GetWhiteScore()
        {
            int whiteScore = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (this[y, x] != null)
                        whiteScore += this[y, x].Team == Team.White ? 1 : 0;
                }
            }
            return whiteScore;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns>black player's score</returns>
        public int GetBlackScore()
        {
            int blackScore = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (this[y, x] != null)
                        blackScore += this[y, x].Team == Team.Black ? 1 : 0;
                }
            }
            return blackScore;
        }
        #endregion
    }
}
