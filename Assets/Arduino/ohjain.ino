#include <OneButton.h>
#define UP_BUTTON 34
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
  Serial.begin(9600);

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

  upButton.tick();
  downButton.tick();
  leftButton.tick();
  rightButton.tick();
  button1.tick();
  button2.tick();

  delay(10);
}

static void UpButtonClick(){
  Serial.println("UP");
}

static void DownButtonClick(){
  Serial.println("DOWN");
}

static void LeftButtonClick(){
  Serial.println("LEFT");    
}

static void RightButtonClick(){
  Serial.println("RIGHT");
}

static void ButtonOneClick(){
  Serial.println("BUTTON1");
}

static void ButtonTwoClick(){
  Serial.println("BUTTON2");
}
