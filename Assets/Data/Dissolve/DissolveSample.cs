using UnityEngine;

public class DissolveSample : MonoBehaviour {

    private Material material;

    private void Start() {
        material = GetComponent<Renderer>().material;
    }

    private void Update() {
        material.SetFloat("_DissolveAmount", Mathf.Sin(Time.time) / 2 + 0.5f);
    }
}