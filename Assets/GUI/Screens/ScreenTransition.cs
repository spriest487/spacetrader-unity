public enum ScreenTransition
{
    FadeToBlack,
    FadeFromBlack,
    FadeOutAlpha,
    FadeInAlpha,
}

public static class ScreenTransitionUtility
{
    public static bool IsShowOverlay(this ScreenTransition transition)
    {
        switch (transition)
        {
            case ScreenTransition.FadeToBlack:
            case ScreenTransition.FadeFromBlack:
                return true;
            default:
                return false;
        }
    }

    public static bool IsShowOverlayAfter(this ScreenTransition transition)
    {
        switch (transition)
        {
            case ScreenTransition.FadeToBlack:
                return true;
            default:
                return false;
        }
    }

    public static bool IsInvertDirection(this ScreenTransition transition)
    {
        switch (transition)
        {
            case ScreenTransition.FadeFromBlack:
            case ScreenTransition.FadeOutAlpha:
                return true;
            default:
                return false;
        }
    }
}
