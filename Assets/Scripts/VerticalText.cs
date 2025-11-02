using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(TMP_Text))]
public class VerticalText : MonoBehaviour
{
    private TMP_Text textComponent;

    // 回転させない文字群
    public List<char> noRotateCharacters;

    // 反転させたい文字群
    public List<char> flipCharacters;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textComponent.ForceMeshUpdate();

        var textInfo = textComponent.textInfo;
        if (textInfo.characterCount == 0)
            return;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible)
                continue;


            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;
            Vector3[] destVertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 charCenter = (destVertices[vertexIndex + 0] + destVertices[vertexIndex + 2]) / 2;
            if (IsFlipCharacter(characterInfo.character))
            {
                // 反転
                for (int j = 0; j < 4; j++)
                {
                    var element = destVertices[vertexIndex + j];
                    var pos = element - charCenter;
                    var newPos = new Vector2(pos.x, -pos.y);

                    element = (Vector3)newPos + charCenter;
                    destVertices[vertexIndex + j] = element;
                }
            }

            if (IsNoRotateCharacter(characterInfo.character))
                continue;

            for (int j = 0; j < 4; j++)
            {
                var element = destVertices[vertexIndex + j];
                var pos = element - charCenter;
                var newPos = new Vector2(
                    pos.x * Mathf.Cos(90 * Mathf.Deg2Rad) - pos.y * Mathf.Sin(90 * Mathf.Deg2Rad),
                    pos.x * Mathf.Sin(90 * Mathf.Deg2Rad) + pos.y * Mathf.Cos(90 * Mathf.Deg2Rad)
                );

                element = (Vector3)newPos + charCenter;
                destVertices[vertexIndex + j] = element;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    bool IsNoRotateCharacter(char c)
    {
        return noRotateCharacters.Contains(c);
    }

    bool IsFlipCharacter(char c)
    {
        return flipCharacters.Contains(c);
    }
}
