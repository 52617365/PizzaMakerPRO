#include <OneButton.h>
#include "BluetoothSerial.h"


#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;

#define UP_BUTTON 32
#define DOWN_BUTTON 33
#define LEFT_BUTTON 25
#define RIGHT_BUTTON 26
#define BUTTON1 27
#define BUTTON2 14
#define BUTTON3 12

// Button setup for OneButton library.
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
OneButton button1 = OneButton(
  BUTTON1,
  true,
  true
  );
OneButton button2 = OneButton(
  BUTTON2,
  true,
  true
  );
OneButton button3 = OneButton(
  BUTTON3,
  true,
  true
  );

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  SerialBT.begin("OHJAIN");

  // Setup for clicks and long presses on buttons that are
  // handled by OneButton.
  upButton.attachClick(UpButtonClick);
  upButton.attachLongPressStop(UpButtonClick);
  downButton.attachClick(DownButtonClick);
  downButton.attachLongPressStop(DownButtonClick);
  leftButton.attachClick(LeftButtonClick);
  leftButton.attachLongPressStop(LeftButtonClick);
  rightButton.attachClick(RightButtonClick);
  rightButton.attachLongPressStop(RightButtonClick);
  button1.attachClick(ButtonOneClick);
  button1.attachLongPressStop(ButtonOneClick);
  button2.attachClick(ButtonTwoClick);
  button2.attachLongPressStop(ButtonTwoClick);
  button3.attachClick(ButtonThreeClick);
  button3.attachLongPressStop(ButtonThreeClick);

  upButton.setClickTicks(200);
  downButton.setClickTicks(200);
  leftButton.setClickTicks(200);
  rightButton.setClickTicks(200);
  button1.setClickTicks(200);
  button2.setClickTicks(200);
  button3.setClickTicks(200);
  
  pinMode(LEFT_BUTTON, INPUT_PULLUP);
  pinMode(RIGHT_BUTTON, INPUT_PULLUP);
  pinMode(UP_BUTTON, INPUT_PULLUP);
  pinMode(DOWN_BUTTON, INPUT_PULLUP);
  pinMode(BUTTON1, INPUT_PULLUP);
  pinMode(BUTTON2, INPUT_PULLUP);
  pinMode(BUTTON3, INPUT_PULLUP);
}

int leftButtonStatus = 0;
int rightButtonStatus = 0;
int upButtonStatus = 0;
int downButtonStatus = 0;

const float maxTimer = 10;
float timeOutTimer = 0;

// Bools for checking if any joystick button is currently being pressed.
bool upInput = false;
bool downInput = false;
bool rightInput = false;
bool leftInput = false;

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available()) {
    SerialBT.write(Serial.read());
  }
  if (SerialBT.available()) {
    Serial.write(SerialBT.read());
  }

  upButton.tick();
  downButton.tick();
  leftButton.tick();
  rightButton.tick();
  button1.tick();
  button2.tick();
  button3.tick();

  if (timeOutTimer >= maxTimer){
    //Serial.println("timeout");
    SerialBT.println("timeout");
    timeOutTimer = 0;
  }

  timeOutTimer += 0.010;
  
  // Jos aiheutuu lagia serialin lukemisessa niin delaytä voi nostaa suuremmaksi.
  delay(10);
  
  if(digitalRead(LEFT_BUTTON) == LOW && leftInput == false){
    // Lähetettävän arvon tulisi olla Horizontal -1
    //Serial.println("1");
    SerialBT.println("1");
    leftInput = true;
  }


  if (digitalRead(RIGHT_BUTTON) == LOW && rightInput == false){
    // Lähetettävän arvon tulisi olla Horizontal 1
    //Serial.println("3");
    SerialBT.println("3");
    rightInput = true;
  }


  if (digitalRead(UP_BUTTON) == LOW && upInput == false){
    // Lähetettävän arvon tulisi olla Vertical 1
    //Serial.println("-3");
    SerialBT.println("-3");
    upInput = true;
  }


  if (digitalRead(DOWN_BUTTON) == LOW && downInput == false){
    // Lähetettävän arvon tulisi olla Vertical -1
    //Serial.println("-1");
    SerialBT.println("-1");
    downInput = true;
  }

  if (leftInput || rightInput || upInput || downInput){
    timeOutTimer = 0;
  }
}

// Lopettaa pelaajan liikkumisen siihen suuntaan mitä nappia ei enää pidetä pohjassa.
static void UpButtonClick(){
  // Lähetettävän arvon tulisi olla Vertical 0
  //Serial.println("-4");
  SerialBT.println("-4");
  upInput = false;
}

static void DownButtonClick(){
  // Lähetettävän arvon tulisi olla Vertical 0
  //Serial.println("-2");
  SerialBT.println("-2");
  downInput = false;
}

static void LeftButtonClick(){
  // Lähetettävän arvon tulisi olla Horizontal 0
  //Serial.println("2");  
  SerialBT.println("2");
  leftInput = false;
}

static void RightButtonClick(){
  // Lähetettävän arvon tulisi olla Horizontal 0
  //Serial.println("4");
  SerialBT.println("4");
  rightInput = false;
}

static void ButtonOneClick(){
  //Serial.println("5");
  SerialBT.println("5");
}

static void ButtonTwoClick(){
  //Serial.println("6");
  SerialBT.println("6");
}

static void ButtonThreeClick(){
  //Serial.println("7");
  SerialBT.println("7");
}
