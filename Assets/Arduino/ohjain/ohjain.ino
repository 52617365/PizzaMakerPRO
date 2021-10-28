#include <OneButton.h>
#include "BluetoothSerial.h"
#define LEFT_BUTTON 27
#define RIGHT_BUTTON 26
#define UP_BUTTON 25
#define DOWN_BUTTON 33
BluetoothSerial SerialBT;

OneButton upButton = OneButton(
  UP_BUTTON,
  true,
  true
  );
OneButton downButton = OneButton(
  DOWN_BUTTON,
  true,
  true
  );
OneButton leftButton = OneButton(
  LEFT_BUTTON,
  true,
  true
  );
OneButton rightButton = OneButton(
  RIGHT_BUTTON,
  true,
  true
  );

  enum {
    /* X AXIS*/
    LEFT = 1, // Starts movement LEFT
    STOPLEFT = 2, // Stops movement LEFT
    
    RIGHT = 3, // Starts movement RIGHT
    STOPRIGHT = 4, // Stops movement RIGHT

    /* Y AXIS */
    DOWN = -1, // Starts movement DOWN
    STOPDOWN = -2, // Stops movement DOWN
    
    UP = -3, // Starts movement UP
    STOPUP = -4 // Stops movement UP
  };

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  SerialBT.begin("RasenESP"); //Bluetooth device name
  Serial.println("Device started");
  
  upButton.attachClick(UpButtonClick);
  upButton.attachLongPressStop(UpButtonClick);
  downButton.attachClick(DownButtonClick);
  downButton.attachLongPressStop(DownButtonClick);
  leftButton.attachClick(LeftButtonClick);
  leftButton.attachLongPressStop(LeftButtonClick);
  rightButton.attachClick(RightButtonClick);
  rightButton.attachLongPressStop(RightButtonClick);

  upButton.setClickTicks(200);
  downButton.setClickTicks(200);
  leftButton.setClickTicks(200);
  rightButton.setClickTicks(200);
  
  pinMode(LEFT_BUTTON, INPUT_PULLUP);
  pinMode(RIGHT_BUTTON, INPUT_PULLUP);
  pinMode(UP_BUTTON, INPUT_PULLUP);
  pinMode(DOWN_BUTTON, INPUT_PULLUP);  
}

int leftButtonStatus = 0;
int rightButtonStatus = 0;
int upButtonStatus = 0;
int downButtonStatus = 0;

void loop() {
  // put your main code here, to run repeatedly:
  int leftPinValue = digitalRead(LEFT_BUTTON);
  int rightPinValue = digitalRead(RIGHT_BUTTON);
  int upPinValue = digitalRead(UP_BUTTON);
  int downPinValue = digitalRead(DOWN_BUTTON);

  upButton.tick();
  downButton.tick();
  leftButton.tick();
  rightButton.tick();

  // Jos aiheutuu lagia serialin lukemisessa niin delaytä voi nostaa suuremmaksi.
  delay(20);

  if(digitalRead(LEFT_BUTTON) == LOW){
    // Lähetettävän arvon tulisi olla Horizontal -1
    SerialBT.write(LEFT);
  }


  if (digitalRead(RIGHT_BUTTON) == LOW){
    // Lähetettävän arvon tulisi olla Horizontal 1
    SerialBT.write(RIGHT);
  }


  if (digitalRead(UP_BUTTON) == LOW){
    // Lähetettävän arvon tulisi olla Vertical 1
    SerialBT.write(UP);
  }


  if (digitalRead(DOWN_BUTTON) == LOW){
    // Lähetettävän arvon tulisi olla Vertical -1
    SerialBT.write(DOWN);
  }

}

// Lopettaa pelaajan liikkumisen siihen suuntaan mitä nappia ei enää pidetä pohjassa.
static void UpButtonClick(){
  // Lähetettävän arvon tulisi olla Vertical 0
  SerialBT.write(STOPUP);
}

static void DownButtonClick(){
  // Lähetettävän arvon tulisi olla Vertical 0
  SerialBT.write(STOPDOWN);
}

static void LeftButtonClick(){
  // Lähetettävän arvon tulisi olla Horizontal 0
  SerialBT.write(STOPLEFT);    
}

static void RightButtonClick(){
  // Lähetettävän arvon tulisi olla Horizontal 0
  SerialBT.write(STOPRIGHT);
}
