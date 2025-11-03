using TMPro;
using UnityEngine;
using System.Collections.Generic;

// TMP_Textコンポーネントのテキストを縦書きにするスクリプト
// 特定の文字を回転させない、または反転させるオプション付き
// 使用例: 明朝体での縦書き表示など
// TODO: 、。促音、拗音の位置調整
[RequireComponent(typeof(TMP_Text))]
public class VerticalText : MonoBehaviour
{
    private TMP_Text textComponent;

    // 回転させない文字群
    // 例: 、。・ーなど
    public List<char> noRotateCharacters;

    // 反転させたい文字群
    // 明朝体のー〜など
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
                FlipCharacter(destVertices, vertexIndex, charCenter);
            }

            if (IsNoRotateCharacter(characterInfo.character))
                continue;

            RotateCharacter(destVertices, vertexIndex, charCenter);
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    bool IsFlipCharacter(char c)
    {
        return flipCharacters.Contains(c);
    }

    bool IsNoRotateCharacter(char c)
    {
        return noRotateCharacters.Contains(c);
    }

    void FlipCharacter(Vector3[] destVertices, int vertexIndex, Vector3 charCenter)
    {
        for (int j = 0; j < 4; j++)
        {
            var pos = destVertices[vertexIndex + j] - charCenter;
            var newPos = new Vector2(pos.x, -pos.y);

            destVertices[vertexIndex + j] = (Vector3)newPos + charCenter;
        }
    }

    void RotateCharacter(Vector3[] destVertices, int vertexIndex, Vector3 charCenter)
    {
        for (int j = 0; j < 4; j++)
        {
            var pos = destVertices[vertexIndex + j] - charCenter;
            var newPos = new Vector2(
                pos.x * Mathf.Cos(90 * Mathf.Deg2Rad) - pos.y * Mathf.Sin(90 * Mathf.Deg2Rad),
                pos.x * Mathf.Sin(90 * Mathf.Deg2Rad) + pos.y * Mathf.Cos(90 * Mathf.Deg2Rad)
            );

            destVertices[vertexIndex + j] = (Vector3)newPos + charCenter;
        }
    }
}
