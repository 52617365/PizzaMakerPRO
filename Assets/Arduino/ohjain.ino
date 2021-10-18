#include "OneButton.h"
#include "BluetoothSerial.h"


#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;

#define UP_BUTTON 15      /*34 TÄMÄ ON VAIHDETTU TESTI MIELESSÄ MINULLE SOPIVAKSI  */
#define DOWN_BUTTON 35
#define LEFT_BUTTON 32
#define RIGHT_BUTTON 33
#define BUTTON1 25
#define BUTTON2 26

OneButton upButton = OneButton(
  UP_BUTTON,
  false,
  false
  );
OneButton downButton = OneButton(
  DOWN_BUTTON,
  false,
  false
  );
OneButton leftButton = OneButton(
  LEFT_BUTTON,
  false,
  false
  );
OneButton rightButton = OneButton(
  RIGHT_BUTTON,
  false,
  false
  );
OneButton button1 = OneButton(
  BUTTON1,
  false,
  false
  );
OneButton button2 = OneButton(
  BUTTON2,
  false,
  false
  );



void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  SerialBT.begin("RasenESP"); //Bluetooth device name
  Serial.println("The device started, now you can pair it with bluetooth!");
  
  upButton.attachClick(UpButtonClick);
  downButton.attachClick(DownButtonClick);
  leftButton.attachClick(LeftButtonClick);
  rightButton.attachClick(RightButtonClick);
  button1.attachClick(ButtonOneClick);
  button2.attachClick(ButtonTwoClick);

  upButton.setClickTicks(200);
  downButton.setClickTicks(200);
  leftButton.setClickTicks(200);
  rightButton.setClickTicks(200);
  button1.setClickTicks(200);
  button2.setClickTicks(200);
}

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

  delay(10);
}

static void UpButtonClick(){
  SerialBT.println("UP");
}

static void DownButtonClick(){
  SerialBT.println("DOWN");
}

static void LeftButtonClick(){
  SerialBT.println("LEFT");    
}

static void RightButtonClick(){
  SerialBT.println("RIGHT");
}

static void ButtonOneClick(){
  Serial.println("BUTTON1");
}

static void ButtonTwoClick(){
  Serial.println("BUTTON2");
}