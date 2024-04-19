var WebSocket = require("ws");

const wss = new WebSocket.Server({ port: 3000});
var alive = true;
var screens = [];
var inventoryData = {inventory:[]}

var green = false;
var blue = false;

/**
 * Tells socket to update inventory with new information
 * @param {string} id socket id
 */
function addToInv(id){
  console.log("add");
  inventoryData.inventory.push(id);
  let send = {"type":"inventoryUpdate","inventory":inventoryData.inventory};
  send = JSON.stringify(send)
  console.log(screens.length)
  for(let x = 0; x<screens.length; x++){
    console.log("sending")
  screens[x].send(send);
  }
  console.log(inventoryData.inventory);
}

function removeFronInv(id){
  console.log("remove");
  console.log(id);
  RemoveItem(id);
  let send = {"type":"inventoryUpdate","inventory":inventoryData.inventory};
  send = JSON.stringify(send)
  //console.log(screens)
  for(let x = 0; x<screens.length; x++){
   
    screens[x].send(send);
  }
  console.log(inventoryData.inventory);
}

/**
 * Sends broadcast to all three screens for intro animation to play across all screens
 */
function StartIntro() {
  let send = {"type":"intro"};
  send = JSON.stringify(send)
  for(let x = 0; x<screens.length; x++){
   
    screens[x].send(send);
  }

}
 /** Tells socket to update inventory with new information
 * @param {string} light light to turn on
 */
function activateLight(light){
  let temp = GetSocketFromID(9);
  console.log("send light"+light);
  console.log(screens.length)
  temp.send(light);

}


function killPhoenix(){
  console.log("kill phoenix");
  console.log("number 3"+GetSocketFromID(3).id);
  let temp = GetSocketFromID(3);
  let send = {"type":"kill","inventory":inventoryData.inventory};
  send = JSON.stringify(send);
  temp.send(send);
}

function checkWin(){
      if(green&&blue){
        let temp = GetSocketFromID(3);
        let send = {"type":"win","inventory":inventoryData.inventory};
        send = JSON.stringify(send);
        temp.send(send);
      }
}

/**
 * Processes request from client
 * @param {JSON} data information sent from client
 * @param {WebSocket} ws WebSocket of client that made request
 */

function processRequest(data,ws){

  if(data.toString()==9){

    keepAlive();
    addScreen(ws);


    /**
     * Send keep alive ping to socket to ensure constant connect (currently only uses for arduino)
     * @param {WebSocket} socket WebSocket to do ping check on
     */
    function pingCheck(socket){ 
      if(!alive){
        let temp = FindSocket(ws);
        temp.close();
        screens.splice(temp,1);
        clearInterval(pingInterval);
        console.log("arduino gone");
        
      }else{
        killAlive();
      }
      socket.send("ping")
    }

   var pingInterval = setInterval(() => {
      pingCheck(ws)
    }, 3000);

    return;

  }
  
    let sent = JSON.parse(data);
    console.log(sent.type);
    switch (sent.type) {
      case "add":
        addToInv(sent.added,ws);
        break;
      case "remove":
        removeFronInv(sent.added,ws);
        console.log("go to remove", sent.added);
        break;

        case "kill":
          killPhoenix();
          console.log("go to phoenix");
          break;

          case "blue":
          blue = true;
          checkWin();
            break;

            case "green":
        green=true;
        checkWin();
        break;

      case "intro":
        StartIntro();
        break;
      case "light":
        activateLight(sent.added);
        break;
    
      default:
        break;
    }
  
}

/**
 * Resets ping
 */
function keepAlive(){
  alive=true;
}

/**
 * Starts keep alive death
 */
function killAlive(){
  alive=false;
}

/**
 * Adds id to newly connected screen
 * @param {WebSocket} ws WebSocket to add id to
 * @param {JSON} data JSON object that provides id information
 */
function addScreen(ws,data){
  if(data==undefined){
    console.log("added 9");
    ws.id=9;
  }else{
  ws.id = data.toString();}
  console.log(ws.id);
  screens.push(ws);


}




/**
 * Returns local reference to socket given online socket
 * @param {WebSocket} socket Socket to find in list
 * @returns {WebSocket} Local reference to socket
 */
function FindSocket(socket){
  for (var x = 0; x < screens.length; x++) {
    if (screens[x] == socket) return screens[x];
  }
}

/**
 * Returns local reference to socket via given ID
 * @param {number} id ID of socket
 * @returns {WebSocket} Socket with given ID
 */
function GetSocketFromID(id) {
  for (var x = 0; x < screens.length; x++) {
    if (screens[x].id == id) return screens[x];
  }
}


/**
 * Returns local reference to socket via given ID
 * @param {number} id ID of item
 * @returns {JSON} Item with given ID
 */
function GetItemFromID(id) {
  for (var x = 0; x < inventoryData.inventory.length; x++) {
    if (inventoryData.inventory[x] == id) return x;
  }
}

/**
 * Removes given WebSocket from local reference, allows it to be readded when socket reconnects
 * @param {WebSocket} ws WebSocket to remove
 */
function KillWebsocket(ws){
  temp = FindSocket(ws)
  screens.splice(temp,1)
}

/**
 * Removes given WebSocket from local reference, allows it to be readded when socket reconnects
 * @param {number} id item to remove
 */
function RemoveItem(id){
  console.log("remove");
  temp = GetItemFromID(id)
  if(temp==null)return;
  inventoryData.inventory.splice(temp,1)
}


wss.on("connection", function connection(ws) {
  console.log("Connected");
  
  ws.on("error", console.error);

 

  ws.on("message", function message(data) {
    try{
      if(JSON.parse(data)){
        processRequest(data,ws)
      }

  }
  catch{}


    if(data.toString()=="pong"){

      keepAlive();

    }
    
    if (ws.id == undefined) {
      addScreen(ws,data);
      return;
    }
    
    if (ws.id == 0) {
      KillWebsocket(ws)
    }


  });

 

});




console.log("READY FOR GAME");