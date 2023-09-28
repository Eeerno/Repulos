using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class MapScript : MonoBehaviour
{
    [SerializeField] RectTransform targetBlinker;
    [SerializeField] RectTransform miniPlane;
    [SerializeField] Transform mapUpperLeftInWorld;
    [SerializeField] Transform mapLowerRightInWorld;
    [SerializeField] GameObject playerObject;
    GameObject targetObject;
    
    private void OnGUI()
    {
        float mapHeightInWorld =    Mathf.Abs( mapLowerRightInWorld.position.z - mapUpperLeftInWorld.position.z );
        float mapWidthInWorld =     Mathf.Abs( mapLowerRightInWorld.position.x - mapUpperLeftInWorld.position.x );
        RectTransform map = this.GetComponent<RectTransform>();
        

        // Set target blinker position on map
        float targetNormalizedZ = ( targetObject.transform.position.z - mapUpperLeftInWorld.position.z ) / mapHeightInWorld;
        float targetNormalizedX = ( targetObject.transform.position.x - mapLowerRightInWorld.position.x ) / mapWidthInWorld;

        targetBlinker.localPosition = new Vector2(  map.rect.x + map.rect.width - ( map.rect.width * targetNormalizedX ),
                                                    map.rect.y + map.rect.height - ( map.rect.height * targetNormalizedZ ));

        // Set player plane position on map
        float playerNormalizedY = ( playerObject.transform.position.z - mapUpperLeftInWorld.position.z ) / mapHeightInWorld;
        float playerNormalizedX = ( playerObject.transform.position.x - mapLowerRightInWorld.position.x ) / mapWidthInWorld;

        miniPlane.localPosition = new Vector2(  map.rect.x + map.rect.width - ( map.rect.width * playerNormalizedX ),
                                                map.rect.y + map.rect.height - ( map.rect.height * playerNormalizedY ));
        
        // Rotate the mini plane on the map
        Vector3 playerRotEuler = playerObject.transform.rotation.eulerAngles;
        Quaternion miniPlaneRotation = Quaternion.Euler ( 0, 0, -playerRotEuler.y + 180 );
        miniPlane.rotation = miniPlaneRotation;

    }

    public void assignNewTarget(GameObject newTarget)
    {
        targetObject = newTarget;
    }
}
