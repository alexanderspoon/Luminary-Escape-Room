/*
 * ArdunioCandelabra.ino
 *
 *  Created on: 2/13/2024
 *
 */

#include <Arduino.h>

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>

#include <WebSocketsClient.h>

#include <Hash.h>

ESP8266WiFiMulti WiFiMulti;
WebSocketsClient webSocket;
int intValue;



// Allocate memory for the string
char *payloadString = nullptr;



void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {

	switch(type) {
		case WStype_DISCONNECTED:
			Serial.print("[WSc] Disconnected!\n");
			break;
		case WStype_CONNECTED: {
			Serial.print(" Connected to url");

			// send message to server when Connected
			webSocket.sendTXT("9");
		}
			break;
		case WStype_TEXT:
            Serial.println("got new message");
           // Check if payloadString is already allocated
            if (payloadString == nullptr) {
                payloadString = new char[length + 1];
            }
            else {
                // Reallocate memory if necessary
                delete[] payloadString;
                payloadString = new char[length + 1];
            }
            // Copy payload to payloadString
            memcpy(payloadString, payload, length);
            // Null-terminate the string
            payloadString[length] = '\0';
            // Print payloadString
            //Serial.println(strcmp(payloadString, "ping") == 0);
            if(strcmp(payloadString, "ping") == 0){
              Serial.println("send ");
              webSocket.sendTXT("pong");
            }


   
    //intValue = *payload - '0';
  Serial.println("payloadString");

  Serial.println(payloadString);

    if(strcmp(payloadString, "1") == 0){
      Serial.println("got here");
      
  // but actually the LED is on; this is because
  // it is active low on the ESP-01)
  digitalWrite(5, HIGH);  // Turn the LED off by making the voltage LOW
    }
Serial.println("number2");
Serial.println(strcmp(payloadString, "ping") == 0);
        if(strcmp(payloadString, "2") == 0){
  // but actually the LED is on; this is because
  // it is active low on the ESP-01)
  digitalWrite(13, HIGH);  // Turn the LED off by making the voltage LOW
    }

      if(strcmp(payloadString, "3") == 0){
  // but actually the LED is on; this is because
  // it is active low on the ESP-01)
  digitalWrite(14, HIGH);  // Turn the LED off by making the voltage LOW
    }

			// send message to server
			// webSocket.sendTXT("message here");

      
			break;
		case WStype_BIN:
			//Serial.print("[WSc] get binary length: %u\n", length);
			hexdump(payload, length);

			// send data to server
			// webSocket.sendBIN(payload, length);
			break;
        case WStype_PING:
            // pong will be send automatically
            Serial.print("[WSc] get ping\n");
            break;
        case WStype_PONG:
            // answer to a ping we send
            Serial.print("[WSc] get pong\n");
            break;
    }

}

void setup() {
	// Serial.begin(921600);
	Serial.begin(115200);

  pinMode(5, OUTPUT);  // Initialize the Red LED pin as an output
  pinMode(13, OUTPUT);  // Initialize the Blue LED pin as an output
  pinMode(14, OUTPUT);  // Initialize the Green LED pin as an output

	//Serial.setDebugOutput(true);
	Serial.setDebugOutput(true);

	Serial.println();
	Serial.println();
	Serial.println();

	for(uint8_t t = 4; t > 0; t--) {
		Serial.printf("[SETUP] BOOT WAIT %d...\n", t);
		Serial.flush();
		delay(1000);
	}
  //Replace with whatever internet login information you are using
	WiFiMulti.addAP("Arduino", "");

	//WiFi.disconnect();
	while(WiFiMulti.run() != WL_CONNECTED) {
		delay(100);
	}


	// server address, port and URL
	webSocket.begin("192.168.160.137", 3000, "/");

	// event handler
	webSocket.onEvent(webSocketEvent);



	// try ever 5000 again if connection has failed
	webSocket.setReconnectInterval(5000);
  
  // start heartbeat (optional)
  // ping server every 15000 ms
  // expect pong from server within 3000 ms
  // consider connection disconnected if pong is not received 2 times
  webSocket.enableHeartbeat(15000, 3000, 2);

}

void loop() {
	webSocket.loop();
}