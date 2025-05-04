using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject[] _currentBlock = new GameObject[3];

    private void Update()
    {
       GameObject block = _currentBlock[1];
        Vector3 blockPosition2D = new Vector3(block.transform.position.x, _playerTransform.position.y, _playerTransform.position.z);
        if( Vector3.Distance(_playerTransform.position, blockPosition2D + Vector3.right *(block.transform.localScale.x/2) ) < 1f) 
        {
          
        }
    }
}
