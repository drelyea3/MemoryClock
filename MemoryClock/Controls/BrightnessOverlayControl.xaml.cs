﻿using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace MemoryClock.Controls
{
    public sealed partial class BrightnessOverlayControl : UserControl
    {
        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dimming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessOverlayControl), new PropertyMetadata(0, OnBrightnessChanged));

        private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dimmer = (BrightnessOverlayControl)d;
            dimmer.overlay.Opacity = 1.0 - dimmer.Brightness;
        }

        const int DEFAULT_DURATION = 5000; // ms

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(int), typeof(BrightnessOverlayControl), new PropertyMetadata(DEFAULT_DURATION, OnDurationChanged));

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dimmer = (BrightnessOverlayControl)d;
            dimmer.InitializeAnimation(dimmer.Duration);
        }

        public BrightnessOverlayControl()
        {
            this.InitializeComponent();

            InitializeAnimation(Duration);
        }

        private void InitializeAnimation(int duration)
        {
            // Initialize implicit animation
            var overlayVisual = ElementCompositionPreview.GetElementVisual(overlay);
            var compositor = overlayVisual.Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.Target = nameof(Visual.Opacity);
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", compositor.CreateLinearEasingFunction());
            animation.Duration = TimeSpan.FromMilliseconds(Duration);
            var animations = compositor.CreateImplicitAnimationCollection();
            animations[animation.Target] = animation;
            overlayVisual.ImplicitAnimations = animations;
        }
    }
}