# GPU Othello
Abdalla Farid et Chacun Guillaume
2018 – 2019, HE-Arc Ingénierie, Neuchâtel

# Introduction
GPU Othello est un projet réalisé pour le cours de .NET du programme Développement Logiciel et Multimédia de la HE-Arc Ingénierie à Neuchâtel. Le but du travail est de reproduire le jeu de plateau Othello. L'implémentation est en C# et en XAML avec WPF. Le cahier des charges requiert l'implémentation des règles du jeu pour qu'il soit jouable à deux joueurs sur un même ordinateur, de l'aperçu des coups jouables, de la sauvegarde et du chargement des parties. Le jeu doit également afficher des horloges représentant le temps de réflexion de chaque joueur, afficher leur score (mis à jour grâce au DataBinding) et s'adapter à n'importe quelle résolution d'écran.

![](https://i.imgur.com/ZdB7yDP.png)
 
# Résultat
L'entièreté du cahier des charges a été rempli. Plusieurs fonctionnalités bonus ont été réalisées :
-	Possibilité de revenir en arrière
-	Animations pour la pose ou le retournement des pièces
-	Magnifique thème présentant l’éternel combat entre AMD et Nvidia
-	Indication du joueur gagnant en fonction de l’arrière-plan
Pour le DataBinding, nous avons décidé d’aller plus loin en expérimentant les fonctionnalités offertes par WPF et ainsi l’implémenter partout où c’était possible en utilisant des MultiBinding et des ValueConverter.
L'application a été testée et aucun bug majeur n'a été répertorié.
