using UnityEngine;

public sealed class GuiUtils
{
    public delegate void GuiDrawAction(bool outlinePass);

    private static void OffsetGuiTranslation(Vector4 baseTranslate, int offsetX, int offsetY)
    {
        var newXlate = new Vector4(
            baseTranslate.x + offsetX,
            baseTranslate.y + offsetY,
            baseTranslate.z,
            baseTranslate.w);

        var matrix = GUI.matrix;
        matrix.SetColumn(3, newXlate);
        GUI.matrix = matrix;
    }

    public static void DrawOutlined(GuiDrawAction action, int distance)
    {
        var oldMatrix = GUI.matrix;

        Vector4 baseTranslation = oldMatrix.GetColumn(3);

        int offsetX = -distance;
        int offsetY = -distance;
        OffsetGuiTranslation(baseTranslation, offsetX, offsetY);
        action(true);

        offsetX = distance;
        OffsetGuiTranslation(baseTranslation, offsetX, offsetY);
        action(true);

        offsetY = distance;
        OffsetGuiTranslation(baseTranslation, offsetX, offsetY);
        action(true);

        offsetX = -distance;
        OffsetGuiTranslation(baseTranslation, offsetX, offsetY);
        action(true);

        GUI.matrix = oldMatrix;
        action(false);       
    }
}
