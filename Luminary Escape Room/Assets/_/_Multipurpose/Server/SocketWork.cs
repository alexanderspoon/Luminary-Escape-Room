using UnityEngine;
using WebSocketSharp;
using TMPro;
using System;
using System.Collections.Generic;

public class SocketWork : MonoBehaviour
{
    WebSocket ws;
    [SerializeField]
    string number;

        private static SocketWork instance;
        private static SocketWork inventoryQueue;

    public GameObject winCanvas;

    private readonly Queue<Action> actionQueue = new Queue<Action>();

    [SerializeField]
    PlayerInventory inventory;
    private void Start()
    {
                if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        ws = new WebSocket("ws://10.31.11.138:3000");
        
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            organizeJSON(e.Data);
            string[] split = e.Data.Split(" ");
            //Debug.Log(split[2]=="1");

            Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
            if(split[2]=="1"){
                Debug.Log("Got here");
                        SocketWork.Enqueue(() => {
            // Enable or modify TextMeshProUGUI here
            //sayHi();
        });
                
            }
        };
        ws.Send(number);
    }
        public void sendData(){
        ws.Send(number);
    }

    public class JSONInfoInvetory{
        public string type;
        public string[] inventory;
    }

        private void organizeJSON(string json){
            Debug.Log(json);
            string[] split = json.Split(",");
            string[] firstSplit = split[0].Split(":");
            //Debug.Log(firstSplit[1]);
            switch (firstSplit[1].Replace("\"",""))
            {
                case "inventoryUpdate":
                    Debug.Log("got new inventory");

                   JSONInfoInvetory inv = JsonUtility.FromJson<JSONInfoInvetory>(json);
                    Debug.Log(inv.inventory.Length);
                        SocketWork.Enqueue(() => {
                            // Enable or modify TextMeshProUGUI here
                            inventory.UpdateInventoryUI(inv.inventory);
                        });
                    

                    break;

                case "kill":
                        Debug.Log("kill phoenix");
                                        SocketWork.Enqueue(() => {
                            // Enable or modify TextMeshProUGUI here
                             inventory.SpawnPhoenixAsh();
                        });
                 
                    

                    break;

                case "win":
                                        SocketWork.Enqueue(() => {
                            // Enable or modify TextMeshProUGUI here
                             winCanvas.SetActive(true);
                        });
                 
                    

                    break;
                default:
                    break;
            }
        }

        void OnApplicationQuit()
    {
        ws.Send("0");
    }

    public void addItem(int id){
        
        ws.Send("{\"type\":\"add\",\"added\":\""+id+"\"}");
    }

        public void killPhoenix(){
            Debug.Log("kill phoenix");
        ws.Send("{\"type\":\"kill\",\"added\":\"0\"}");
    }

    public void removeItem(int id){
        ws.Send("{\"type\":\"remove\",\"added\":\""+id+"\"}");
    }

    public void winBlue(){
        ws.Send("{\"type\":\"blue\",\"added\":\"0\"}");
    }
    public void winGreen(){
        ws.Send("{\"type\":\"green\",\"added\":\"0\"}");
    }



        public static void Enqueue(Action action)
    {
        lock (instance.actionQueue)
        {
            instance.actionQueue.Enqueue(action);
        }
    }
    private void Update()
    {
                lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                actionQueue.Dequeue().Invoke();
            }
        }


        if (ws == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ws.Send("2");
        }
    }



}