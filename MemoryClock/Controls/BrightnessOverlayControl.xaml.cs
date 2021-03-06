﻿using Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MemoryClock.Controls
{
    public sealed partial class BrightnessOverlayControl : UserControl
    {
        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessOverlayControl), new PropertyMetadata(0, OnBrightnessChanged));

        private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dimmer = (BrightnessOverlayControl)d;
            dimmer.overlay.Opacity = 1.0 - dimmer.Brightness;
        }

        static readonly TimeSpan DEFAULT_DURATION = TimeSpan.FromSeconds(1);

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(BrightnessOverlayControl), new PropertyMetadata(DEFAULT_DURATION, OnDurationChanged));

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dimmer = (BrightnessOverlayControl)d;
            dimmer.SetFader(dimmer.Duration);
        }

        public BrightnessOverlayControl()
        {
            this.InitializeComponent();

            overlay.SetFader(Duration);
        }
    }
}
