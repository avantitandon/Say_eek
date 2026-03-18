using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class StickerOverlayController : MonoBehaviour
{
    private const int T1_THRESHOLD = 50;
    private const int T2_THRESHOLD = 100;
    private const int T3_THRESHOLD = 200;

    private class StickerPlacement {
        public Vector2 position;
        public float radius;
    }

    private List<StickerPlacement> stickerPlacements = new List<StickerPlacement>();

    private int leftCount = 0;
    private int rightCount = 0;

    [SerializeField] private GameObject heartsprite;
    // [SerializeField] private GameObject firesprite;
    // [SerializeField] private GameObject starsprite;
    [SerializeField] private GameObject emptyheart;
    [SerializeField] private GameObject heartCanvas;

    void Start() {
        // 
    }

    public void DisplayStickerOverlay(int score) {
        GenerateStickers(score);
    }

    private bool IsValidPlacement(Vector2 candidatePos, float candidateRadius)
    {
        foreach (var placement in stickerPlacements)
        {
            if (Vector2.Distance(candidatePos, placement.position) < candidateRadius + placement.radius) {
                return false;
            }
        }
        return true;
    }

    private Vector2 GeneratePoint(float candidateRadius) {
        RectTransform canvasRect = heartCanvas.GetComponent<RectTransform>();
        float x1;
        float x2;
        float leftProbability;
        float x;
        float y;
        bool leftSide;

        // try 20 times to find valid placement
        for (int i = 0; i < 20; i++) {
            x1 = UnityEngine.Random.Range(-900f, -700f);
            x2 = UnityEngine.Random.Range(700f, 900f);
            
            leftProbability = 0.5f;
            if (leftCount < rightCount) {
                leftProbability = 0.8f; // increase probability of spawning on the left
            } else if (rightCount < leftCount) {
                leftProbability = 0.2f;
            }
            
            leftSide = UnityEngine.Random.Range(0f, 1f) < leftProbability;
            if (leftSide) {
                x = x1;
            } else {
                x = x2;
            }

            y = UnityEngine.Random.Range(-canvasRect.rect.height / 3f, canvasRect.rect.height / 3f);
            Vector2 candidatePos = new Vector2(x, y);

            if (IsValidPlacement(candidatePos, candidateRadius)) {
                stickerPlacements.Add(new StickerPlacement { position = candidatePos, radius = candidateRadius });
                if (leftSide) leftCount++; else rightCount++;
                return candidatePos;
            }
        }

        // fallback
        x1 = UnityEngine.Random.Range(-1100f, -800f);
        x2 = UnityEngine.Random.Range(800f, 1100f);
        
        leftProbability = 0.5f;
        if (leftCount < rightCount) {
            leftProbability = 0.8f; // increase probability of spawning on the left
        } else if (rightCount < leftCount) {
            leftProbability = 0.2f;
        }
        
        leftSide = UnityEngine.Random.Range(0f, 1f) < leftProbability;
        if (leftSide) {
            x = x1;
            leftCount++;
        } else {
            x = x2;
            rightCount++;
        }
        y = UnityEngine.Random.Range(-canvasRect.rect.height / 3f, canvasRect.rect.height / 3f);
        return new Vector2(x, y);
    }

    // variable sticker scale
    private float varyScale(GameObject sticker, int relative_score)
    {
        float size = UnityEngine.Random.Range(0.8f, relative_score);
        sticker.transform.localScale = new Vector3(size, size, 1f);
        return size;
    }

    // variable sticker Orientation
    private void varyOrientation(GameObject sticker)
    {
        float angle = UnityEngine.Random.Range(-30f, 30f);
        Vector3 rotation = sticker.transform.localEulerAngles;
        rotation.z = angle;
        sticker.transform.localEulerAngles = rotation;
    }

    private GameObject getStickerPrefab()
    {
        // if (ghostScore > 200) return Instantiate(starsprite, emptyheart.transform, false);
        // else if (ghostScore > 100) return Instantiate(heartsprite, emptyheart.transform, false);
        // else return Instantiate(firesprite, emptyheart.transform, false);
        return Instantiate(heartsprite, emptyheart.transform, false);
    }

    private void GenerateStickers(int score) {
        int count = 0;

        Debug.LogError("Ghost Score: " + score);

        if (score > T3_THRESHOLD) count = 3;
        else if (score > T2_THRESHOLD) count = 2;
        else if (score > T1_THRESHOLD) count = 1;

        for (int i = 0; i < count; i++) {
            GameObject stickerObject = getStickerPrefab();

            float scale = varyScale(stickerObject, count);
            varyOrientation(stickerObject);
            float radius = 70f * scale;

            Vector2 pos = GeneratePoint(radius);
            RectTransform rt = stickerObject.GetComponent<RectTransform>();

            rt.anchoredPosition = pos;
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0f);
            //rt.position.z = 0;

            stickerObject.transform.SetAsLastSibling();
            stickerObject.SetActive(true);
        }
    }
}
