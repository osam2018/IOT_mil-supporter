#include <Wire.h>

#define LCDSLAVE 10
#define REFRASH 10
#define LCD 20
#define AIRCONDITIONER 30

int restroomSLAVE[] = {20};
int remoteSLAVE[] = {30};

void blink();
void restroomCommunication(int);
void remoteCommunication(int);
void restroomWire();
void LCDWire();
void remoteWire();
void AIRCONWire();
void refresh();

void setup(){
	Wire.begin();
	pinMode(13, OUTPUT);
	Serial.begin(9600);
}

void loop(){
	if(Serial.available()){
		switch(Serial.parseInt()){
			case REFRASH:
				refresh();
				break;
			case LCD:
				LCDWire();
				break;
			case AIRCONDITIONER:
				AIRCONWire();
				break;
			defult:
				Serial.println("CMD ERR");
		}
	}
}

void refresh(){
	restroomWire();
	remoteWire();
}

void AIRCONWire(){
	int i = -1;
	if(Serial.available()){
		delay(50);
		i = Serial.parseInt();
		if(i < (sizeof(remoteSLAVE) / sizeof(int)) && Serial.available()){
			Wire.beginTransmission(remoteSLAVE[i]);
			Wire.write(Serial.parseInt());
			blink();
			Wire.endTransmission(remoteSLAVE[i]);
		}else{
			Serial.print("CMD ERR REM ");
			Serial.println(i);
		}
	}else{
		Serial.print("CMD ERR REM ");
		Serial.println(i);
	}
}

void remoteWire(){
	for(int i = 0; i < sizeof(remoteSLAVE) / sizeof(int); i++)
		remoteCommunication(i);
}

void LCDWire(){
	if(Serial.available()){
		delay(50);
		Wire.beginTransmission(LCDSLAVE);
		while(Serial.available() > 0){
			Wire.write(Serial.read());
			blink();
		}
		Wire.endTransmission(LCDSLAVE);
	}else
		Serial.println("CMD ERR LCD");
}

void restroomWire(){
	for(int i = 0; i < sizeof(restroomSLAVE) / sizeof(int); i++)
		restroomCommunication(i);
}

void remoteCommunication(int i){
	if(Wire.requestFrom(remoteSLAVE[i], 4)){
		Serial.print("CMD REM ");
		Serial.print(i);
		while(Wire.available() > 0){
			Serial.print(" ");
			Serial.print(Wire.read());
		}
	}else{
		Serial.print("CMD ERR REM ");
		Serial.print(i);
	}
	Serial.println("");
}

void restroomCommunication(int i){
	if(Wire.requestFrom(restroomSLAVE[i], 1)){
		Serial.print("CMD TLT ");
		Serial.print(i);
		Serial.print(" ");
		while(Wire.available() > 0)
			Serial.print(Wire.read());
	}else{
		Serial.print("CMD ERR TLT ");
		Serial.print(i);
	}
	Serial.println("");
}

void blink(){
	digitalWrite(13, HIGH);
	delay(50);
	digitalWrite(13, LOW);
	delay(50);
}
