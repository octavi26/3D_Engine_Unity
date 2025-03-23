using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    private ScreenGenerator script;

    const int MAX = 100;
    public float[,] LightLeveles = new float[9 * MAX,16 * MAX];

    public GameObject light;
    private Vector3 lightPosition;

    private int colums;
    private int rows;

    public int samples = 30;

    public enum ShapeConfig{
        Sphere,
        Cube,
        Torus
    }
    public ShapeConfig shape;
    private ShapeConfig lastShape;

    public float size = 4;
    public float thicknes = 3;
    public Vector3 rotation = new Vector3(0f, 0f, 0f);

    private int lastSamples = 30;
    private float lastSize = 0;
    public float lastThicknes = 3;
    private Vector3 lastRotation = new Vector3(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        script = gameObject.GetComponent<ScreenGenerator>();
        lightPosition = light.transform.position;
        // GenerateShape();
    }

    // Update is called once per frame
    void Update()
    {
        if(ChangeShape()){
            GenerateShape();
        }
    }

    void GenerateShape(){
        colums=script.colums;
        rows = script.rows;

        Debug.Log("Rendering Screen");

        for( int i = 0; i < rows; i++ )
            for( int j = 0; j < colums; j++ ){
                script.Pixels[i,j].GetComponent<SpriteRenderer>().color = Color.black;
                LightLeveles[i,j] = -1;
            }

        float minLL = +10000;
        float maxLL = -10000;
        for( int i = 0; i < rows; i++ )
            for( int j = 0; j < colums; j++ ){
                Vector3 startPosition = gameObject.transform.position;
                Vector3 endPosition = script.Pixels[i,j].transform.position;
                
                Vector3 contactPoint = SamplePoints(startPosition, endPosition);
                if(contactPoint != new Vector3(100,100,100)){
                    // script.Pixels[i,j].GetComponent<SpriteRenderer>().color = Color.white;
                    LightLeveles[i,j] = -2f;

                    bool obstructed = false;
                    for( int k = 1; k < samples; k++ ){
                        Vector3 newPosition = (k * lightPosition + (samples - k) * contactPoint) / samples;
                        if(InsideShape(newPosition)) obstructed = true;
                    }

                    if(!obstructed){
                        LightLeveles[i,j] = (lightPosition - contactPoint).magnitude;
                        minLL = Mathf.Min(minLL, LightLeveles[i,j]);
                        maxLL = Mathf.Max(maxLL, LightLeveles[i,j]);
                    }
                }
            }

        for( int i = 0; i < rows; i++ )
            for( int j = 0; j < colums; j++ ){
                if(LightLeveles[i,j] == -2) LightLeveles[i,j] = maxLL + 1;
                if(LightLeveles[i,j] == -1) continue;

                float normalLightLevel = 1 - (LightLeveles[i,j] - minLL) / (maxLL - minLL);
                script.Pixels[i,j].GetComponent<SpriteRenderer>().color = Color.white * normalLightLevel;
            }
    }

    Vector3 SamplePoints(Vector3 position, Vector3 targetPosition){        
        for( int k = 1; k < samples; k++ ){
            Vector3 lastPosition = ((k-1) * targetPosition + (samples - (k-1)) * position) / samples;
            Vector3 newPosition = (k * targetPosition + (samples - k) * position) / samples;
            if(InsideShape(newPosition)) return lastPosition;
        }
        return new Vector3(100,100,100);
    }

    bool ChangeShape(){
        bool change = false;
        if( lastShape != shape ) change = true;
        if( lastSamples != samples ) change = true;
        if( lastSize != size ) change = true;
        if( lastThicknes != thicknes ) change = true;
        if( lastRotation != rotation ) change = true;
        lastShape = shape;
        lastSamples = samples;
        lastSize = size;
        lastThicknes = thicknes;
        lastRotation = rotation;
        return change;
    }

    bool Sphere(Vector3 position){
        return (position.sqrMagnitude <= size*size);
    }

    bool Cube(Vector3 position){
        return Mathf.Abs(position.x) <= size / 2 &&
            Mathf.Abs(position.y) <= size / 2 &&
            Mathf.Abs(position.z) <= size / 2;
    }

    bool Torus(Vector3 position){
        Vector3 pos2 = new Vector3(position.x, position.y, 0);

        float angle = Mathf.Atan2(position.y, position.x);
        Vector3 pointOnCircle = size * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        // return (position-pointOnCircle).sqrMagnitude <= thicknes * thicknes;
        return (position - pointOnCircle).sqrMagnitude <= thicknes * thicknes;
    }

    bool InsideShape(Vector3 position)
    {
        // Convert rotation angles to radians
        float radX = rotation.x * Mathf.Deg2Rad;
        float radY = rotation.y * Mathf.Deg2Rad;
        float radZ = rotation.z * Mathf.Deg2Rad;

        // Apply INVERSE rotation: rotate in reverse order (Z -> Y -> X)
        position = RotateZ(position, -radZ);
        position = RotateY(position, -radY);
        position = RotateX(position, -radX);

        if(shape == ShapeConfig.Sphere) return Sphere(position);
        if(shape == ShapeConfig.Cube) return Cube(position);
        if(shape == ShapeConfig.Torus) return Torus(position);

        return false;
    }

    // Rotation function around X-axis
    Vector3 RotateX(Vector3 p, float angle)
    {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        return new Vector3(
            p.x,
            cosA * p.y - sinA * p.z,
            sinA * p.y + cosA * p.z
        );
    }

    // Rotation function around Y-axis
    Vector3 RotateY(Vector3 p, float angle)
    {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        return new Vector3(
            cosA * p.x + sinA * p.z,
            p.y,
            -sinA * p.x + cosA * p.z
        );
    }

    // Rotation function around Z-axis
    Vector3 RotateZ(Vector3 p, float angle)
    {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        return new Vector3(
            cosA * p.x - sinA * p.y,
            sinA * p.x + cosA * p.y,
            p.z
        );
    }
}
