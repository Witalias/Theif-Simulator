using UnityEngine;

public class VariantSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] variants;

    //private void Awake()
    //{
    //    var randomVariant = variants[Random.Range(0, variants.Length)];
    //    Instantiate(randomVariant, transform.position, transform.rotation, transform.parent);
    //    Destroy(gameObject);
    //}
}
