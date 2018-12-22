using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace Common
{
    public static class CompositionHelpers
    {
        public static void SetFader(this FrameworkElement element, TimeSpan duration)
        {
            // Initialize implicit animation on opacity
            var visual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = visual.Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();

            animation.Target = nameof(Visual.Opacity);
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", compositor.CreateLinearEasingFunction());
            animation.Duration = duration;

            var animations = visual.ImplicitAnimations ?? (visual.ImplicitAnimations = compositor.CreateImplicitAnimationCollection());
            animations[animation.Target] = animation;
        }

        public static void SetMover(this FrameworkElement element, TimeSpan duration)
        {           
            // Initialize implicit animation on translation
            var visual = ElementCompositionPreview.GetElementVisual(element);
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var compositor = visual.Compositor;

            var animation = compositor.CreateVector3KeyFrameAnimation();
            animation.Target = nameof(Visual.Offset);
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", compositor.CreateLinearEasingFunction());
            animation.Duration = duration;

            var animations = visual.ImplicitAnimations ?? (visual.ImplicitAnimations = compositor.CreateImplicitAnimationCollection());
            animations[animation.Target] = animation;
        }
    }
}
