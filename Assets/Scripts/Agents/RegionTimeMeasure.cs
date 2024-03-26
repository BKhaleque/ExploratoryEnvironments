using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class RegionTimeMeasure : MonoBehaviour
{
    [HideInInspector]
    public string path;

    public Dictionary<(int,int),float> regionTime = new Dictionary<(int,int),float>();
    // Start is called before the first frame update
    void Awake()
    {
        regionTime = new Dictionary<(int,int),float>();
        for(int i = 0; i< 8; i++){
              for(int x = 0; x< 8; x++){
                    regionTime.Add((i,x),0f);
        
              }
        }
    }
    public void initialiseDict(){
        regionTime = new Dictionary<(int,int),float>();
            for(int i = 0; i< 8; i++){
              for(int x = 0; x< 8; x++){
                    regionTime.Add((i,x),0f);

              }
        }
    }
    // public int getPositionCount(){
    //     return positions.Count;
    // }
    void FixedUpdate()
    {
        if(transform.position.x <= 50 && transform.position.z <=50 ){
            regionTime[(0,0)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >50 && transform.position.z <=100 ){
            regionTime[(0,1)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >100 && transform.position.z <=150 ){
            regionTime[(0,2)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >150 && transform.position.z <=200 ){
            regionTime[(0,3)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >200 && transform.position.z <=250 ){
            regionTime[(0,4)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >250 && transform.position.z <=300 ){
            regionTime[(0,5)]+= Time.deltaTime;
        }
        if(transform.position.x <= 50 && transform.position.z >300 && transform.position.z <=350 ){
            regionTime[(0,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z <=50){
            regionTime[(1,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(1,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(1,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(1,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(1,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(1,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 50 && transform.position.x <=100  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(1,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z <=50 && transform.position.z >0){
            regionTime[(2,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(2,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(2,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(2,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(2,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(2,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 100 && transform.position.x <=150  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(2,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z <=50 && transform.position.z >0){
            regionTime[(3,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(3,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(3,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(3,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(3,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(3,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 150 && transform.position.x <=200  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(3,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z <=50 && transform.position.z >0){
            regionTime[(4,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(4,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(4,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(4,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(4,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(4,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 200 && transform.position.x <=250  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(4,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z <=50 && transform.position.z >0){
            regionTime[(5,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(5,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(5,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(5,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(5,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(5,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 250 && transform.position.x <=300  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(5,6)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z <=50  && transform.position.z >0){
            regionTime[(6,0)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >50 && transform.position.z <=100){
            regionTime[(6,1)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >100   && transform.position.z <=150){
            regionTime[(6,2)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >150   && transform.position.z <=200){
            regionTime[(6,3)]+= Time.deltaTime;
        }        
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >200   && transform.position.z <=250){
            regionTime[(6,4)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >250   && transform.position.z <=300){
            regionTime[(6,5)]+= Time.deltaTime;
        }
        if(transform.position.x > 300 && transform.position.x <=350  && transform.position.z >300   && transform.position.z <=350){
            regionTime[(6,6)]+= Time.deltaTime;
        }




    
    }
    static void lineChanger(string newText, string fileName, int line_to_edit)
{
     string[] arrLine = File.ReadAllLines(fileName);
     arrLine[line_to_edit - 1] = newText;
     File.WriteAllLines(fileName, arrLine);
}


    public void writeToFile(){
            //print(regionTime);
            // foreach (var kv in regionTime ){
            //     Debug.Log("Time spent in region " + kv.Key + " is " + kv.Value + " seconds");
            // }
            if(!File.Exists(path))
                File.CreateText(path);
            else if(File.Exists(path))
                System.IO.File.WriteAllText(path,string.Empty);
            // else if (File.Exists(path) && File.ReadAllLines(path).Length >0){
        
            //     for (int i =1; i<= 7; i++){
            //         for (int x = 1; x<7; x++){
            //             var line = (i-1)*50 + "," + (x-1)*50 + "," + regionTime[(i-1,x-1)];
            //             lineChanger(line, path, i+x);
            //         }
            //     }
            //    // File.Close(path);
                var writer = File.AppendText(path);
                for (int i =1; i<= 7; i++){
                    for (int x = 1; x<=7; x++){
                        var line = (i-1)*50 + "," + (x-1)*50 + "," + regionTime[(i-1,x-1)];
                        writer.WriteLine(line);
                    }
                }
                writer.Close();
                //File.Close(path);
            
        
        
    }
}
    // [InitializeOnLoad]
    // public static class PlayStateNotifier
    // {
          
    //     static PlayStateNotifier()
    //         {
    //           EditorApplication.playModeStateChanged += ModeChanged;
    //       }
      
    //       static void ModeChanged(PlayModeStateChange playModeState)
    //       {
    //           if (playModeState == PlayModeStateChange.EnteredEditMode) 
    //           {
    //               Debug.Log("Entered Edit mode.");
    //               foreach (var kv in regionTime ){
                    
    //               }
    //           }
    //       }
    //   }