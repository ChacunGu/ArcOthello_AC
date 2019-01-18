using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace ArcOthello_AC
{
    /// <summary>
    /// A ContentControl that animates the transition between content
    /// Taken and adapted from https://www.codeproject.com/Articles/136786/Creating-an-Animated-ContentControl
    /// </summary>
    [TemplatePart(Name = "PART_PaintArea", Type = typeof(Shape)),
	 TemplatePart(Name = "PART_MainContent", Type = typeof(ContentPresenter))]
	public class AnimatedContentControl : ContentControl
	{
		#region Generated static constructor
		static AnimatedContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentControl), new FrameworkPropertyMetadata(typeof(AnimatedContentControl)));
		}
		#endregion

		Shape m_paintArea;
		ContentPresenter m_mainContent;

		/// <summary>
		/// This gets called when the template has been applied and we have our visual tree
		/// </summary>
		public override void OnApplyTemplate()
		{
			m_paintArea = Template.FindName("PART_PaintArea", this) as Shape;
			m_mainContent = Template.FindName("PART_MainContent", this) as ContentPresenter;

			base.OnApplyTemplate();
		}

		/// <summary>
		/// This gets called when the content we're displaying has changed
		/// </summary>
		/// <param name="oldContent">The content that was previously displayed</param>
		/// <param name="newContent">The new content that is displayed</param>
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if (m_paintArea != null && m_mainContent != null)
			{
				m_paintArea.Fill = CreateBrushFromVisual(m_mainContent);

                // Used when flipping a piece
                if (newContent != null && newContent is Image && ((Image)newContent).Tag != null)
                    BeginAnimationPieceFlip();
                else // Used when posing a piece
                {
                    BeginAnimationFade(1, 0, 0.5, m_paintArea);
                    BeginAnimationFade(0, 1, 0.5, m_mainContent);
                }

            }
			base.OnContentChanged(oldContent, newContent);
		}

		/// <summary>
		/// Starts the animation for the new content
		/// </summary>
		private void BeginAnimationPieceFlip()
		{
			var newContentTransform = new ScaleTransform();
			var oldContentTransform = new ScaleTransform();
			m_paintArea.RenderTransform = oldContentTransform;
			m_mainContent.RenderTransform = newContentTransform;
			m_paintArea.Visibility = Visibility.Visible;
			
			newContentTransform.BeginAnimation(ScaleTransform.ScaleXProperty, CreateAnimation(0, 1));
			oldContentTransform.BeginAnimation(ScaleTransform.ScaleXProperty, CreateAnimation(1, 0, (s, e) => m_paintArea.Visibility = Visibility.Hidden));
		}

        /// <summary>
		/// Starts the animation to fade a piece
		/// </summary>
		private void BeginAnimationFade(double from, double to, double time, FrameworkElement control)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(time))
            };
            control.BeginAnimation(OpacityProperty, da);
        }

        /// <summary>
        /// Creates the animation that moves content in or out of view.
        /// </summary>
        /// <param name="from">The starting value of the animation.</param>
        /// <param name="to">The end value of the animation.</param>
        /// <param name="whenDone">(optional) A callback that will be called when the animation has completed.</param>
        private AnimationTimeline CreateAnimation(double from, double to, EventHandler whenDone = null)
		{
			IEasingFunction ease = new BackEase { Amplitude = 0.25, EasingMode = EasingMode.EaseOut };
			var duration = new Duration(TimeSpan.FromSeconds(0.5));
			var anim = new DoubleAnimation(from, to, duration) { EasingFunction = ease };
			if (whenDone != null)
				anim.Completed += whenDone;
			anim.Freeze();
			return anim;
		}

		/// <summary>
		/// Creates a brush based on the current appearnace of a visual element. The brush is an ImageBrush and once created, won't update its look
		/// </summary>
		/// <param name="v">The visual element to take a snapshot of</param>
		private Brush CreateBrushFromVisual(Visual v)
		{
			if (v == null)
				throw new ArgumentNullException("v");
			var target = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			target.Render(v);
			var brush = new ImageBrush(target);
			brush.Freeze();
			return brush;
		}
	}
}
