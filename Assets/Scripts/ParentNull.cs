using UnityEngine;

public class ParentNull : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stair"))
        {
            transform.parent = null;
            
            GetComponent<Rigidbody>().isKinematic = false;
            
            GetComponent<CapsuleCollider>().isTrigger = false;
        }
    }
}