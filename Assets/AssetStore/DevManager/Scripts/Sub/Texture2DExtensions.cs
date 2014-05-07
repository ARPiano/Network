using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class Texture2DExtensions 
{
	public enum Rotation { Left, Right, HalfCircle }
	public static void Rotate(this Texture2D texture, Rotation rotation)
	{
		Color32[] originalPixels = texture.GetPixels32();
		IEnumerable<Color32> rotatedPixels;
		
		if (rotation == Rotation.HalfCircle)
			rotatedPixels = originalPixels.Reverse();
		else
		{ 
			// Rotate left:
			var firstRowPixelIndeces = Enumerable.Range(0, texture.height).Select(i => i * texture.width).Reverse().ToArray();
			rotatedPixels = Enumerable.Repeat(firstRowPixelIndeces, texture.width).SelectMany(
				(frpi, rowIndex) => frpi.Select(i => originalPixels[i + rowIndex])
				);
			
			if (rotation == Rotation.Right)
				rotatedPixels = rotatedPixels.Reverse();
		}
		
		texture.SetPixels32( rotatedPixels.ToArray() );
		texture.Apply();
	}
}