using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFontMeshWriter : ProceduralMeshFilter {
    public LineFontMeshWriter(IProceduralMeshFilter next):base(next)
    { }

    public const float LineHeight = 32 + 2;
    public const float TabWidth = 4;
    public static Vector2 TextSize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Vector2.zero;
        }
        Nullable<float> width = null;
        Nullable<float> height = null;
        ProcessText(text, (Vector2 position, LineChar lineChar) => {
            float right = position.x + lineChar.Width;
            float up = -position.y + LineHeight;

            if (!width.HasValue) {
                width = right;
            } else {
                if (right > width.Value) {
                    width = right;
                }
            }

            if (!height.HasValue) {
                height = up;
            } else {
                var newHeight = up;
                if (newHeight > height.Value) {
                    height = newHeight;
                }
            }
        });
        if (!width.HasValue || !height.HasValue)
        {
            return Vector2.zero;
        }
        return new Vector2(width.Value, height.Value);
        
    }

    public static void ProcessText(string text, Action<Vector2, LineChar> handler)
    {
        float tabLength = TabWidth * LineFont.Chars[0].Width;

        float x = 0.0f;
        float y = 0.0f;

        char firstChar = (char)32;
        char lastChar = (char)(firstChar + LineFont.Chars.Count - 1);

        for (int i = 0; i < text.Length; ++i) {
            char charID = text[i];

            if (charID >= firstChar && charID <= lastChar) {
                int p = charID - firstChar;

                var vChar = LineFont.Chars[p];

                handler(new Vector2(x, y), vChar);
                x += vChar.Width;
                continue;
            }


            if (charID == '\n') {
                x = 0;
                y -= LineHeight;
                continue;
            }

            if (charID == '\t') {
                x += tabLength;
                continue;
            }
        }
    }


    public LineFontMeshWriter WriteText(string text)
    {
        ProcessText(text, (Vector2 position, LineChar lineChar) =>
        {
            for (int j = 0; j < lineChar.Vertices.Count; j++) {
                Next.Vertex(lineChar.Vertices[j] + position);
                //appendVertex(position, lineFont[p].vertices[i] + ScreenOffsetType(x, y));
            }
        });

        return this;
    }
}
