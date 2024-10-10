using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public static class MiscFunctions
{
    private const int labelBorderSize = 5;
    private const int imageBorderSize = 20;
    private const int imageHeight = 150;
    
    public static void ShuffleArray<T>(T[] array)
    {
        Random random = new Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public static void StyliseLabel(ref Label label)
    {
        label.style.borderTopWidth = labelBorderSize;
        label.style.borderBottomWidth = labelBorderSize;
        label.style.color = Color.cyan;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
    }
    
    public static void StyliseImage(ref Image image)
    {
        image.scaleMode = ScaleMode.ScaleToFit;
        image.style.height = imageHeight;
        image.style.borderBottomWidth = imageBorderSize;
    }
}
