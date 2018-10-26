#include <Wire.h>
#include <LiquidCrystal_I2C.h>

#define SLAVE 10

String receivedMassage;

LiquidCrystal_I2C lcd(0x3F, 16, 2);
boolean isLCDLoading = true;

void blink();
void play();
void LCDManager();
void receiveFromMaster(int);

void setup(){
	receivedMassage = String();
	isLCDLoading = true;
	lcd.begin();
	lcd.backlight();
	lcd.print("Now Loading ...");
	Wire.begin(SLAVE);
	Wire.onReceive(receiveFromMaster);
	pinMode(13, OUTPUT);
}

void loop(){
	if(receivedMassage != String())	play();
}

void receiveFromMaster(int bytes) {
	while(Wire.available() > 0)
		receivedMassage = String(receivedMassage + (char)Wire.read());
	blink();
}

void LCDManager(){
	if(isLCDLoading){
		lcd.clear();
		lcd.print("Toilet Status");
		lcd.setCursor(0,1);
		lcd.print("Available : ");
		isLCDLoading = false;
	}
	lcd.setCursor(11,1);
	lcd.print(receivedMassage + "     ");
}

void play() {
	LCDManager();
	receivedMassage = String();
}

void blink(){
	digitalWrite(13, HIGH);
	delay(50);
	digitalWrite(13, LOW);
	delay(50);
}
