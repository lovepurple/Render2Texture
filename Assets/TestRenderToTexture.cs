using UnityEngine;
using System.Collections.Generic;

public class TestRenderToTexture : MonoBehaviour {

    Camera textureCamera;
    public RenderTexture texture;
    Texture2D texture2D;
    int layer = 7;
    const int resolution = 32;
    List<GameObject> screen = new List<GameObject>(resolution*resolution);
    GameObject cube;

	// Use this for initialization
	void Start () {
        Camera.main.cullingMask = 1 << 0;
        createScene();
        createScreen();
	}

    void createScene() {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = layer;
        var textureCameraObject = new GameObject("texture camera");

        texture2D = new Texture2D(resolution, resolution);
        texture = new RenderTexture(resolution, resolution, 24);
        textureCamera = textureCameraObject.AddComponent<Camera>();
        textureCamera.cullingMask = 1 << 7;
        textureCameraObject.transform.position = new Vector3(1f, 1f, 1f);
        textureCameraObject.transform.LookAt(cube.transform);
        textureCamera.targetTexture = texture;
    }

    void createScreen() {
        var lightGameObject = new GameObject("screen light");
        lightGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.one);
        var light = lightGameObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.2f;

        var parent = new GameObject("screen");
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.parent = parent.transform;
                screen.Add(go);
            }
        }
    }

    void updateScreen() {
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                var go = screen[index(x, y)];
                var color = GetPixel(x, y);
                go.transform.position = Vector3.Lerp(go.transform.position, new Vector3(x, y, (color.r + color.g + color.b) * -2f), Time.deltaTime * 2f);
                go.renderer.material.color = Color.Lerp(go.renderer.material.color, color, Time.deltaTime * 0.1f);
            }
        }
    }

    void renderObject() {
        RenderTexture.active = texture;
        textureCamera.Render();
        texture2D.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        RenderTexture.active = null;
    }

    int index(int x, int y) {
        return x + y * resolution;
    }

    Color GetPixel(int x, int y) {
        return texture2D.GetPixel(x, y);
    }

    // Update is called once per frame
    void Update() {
        cube.transform.localRotation = Quaternion.AngleAxis(Time.time * 10f, Vector3.one);
        renderObject();
        updateScreen();
    }
}
