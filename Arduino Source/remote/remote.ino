#include <Arduino.h>
#include <Wire.h>
#include <DHT11.h>
#include <IRremote.h>

#define SLAVE 30

int PIN = 2;
int LEDB = 9;  // RGB LED의 파란색 핀을 디지털 9번 연결
int LEDR = 11;  // RGB LED의 붉은색 핀을 디지털 11번 연결
float temp = 0;
float humi = 0;
float discomfort = 0;
boolean Power = false;

DHT11 dht11(PIN);
IRsend irsend;

void blink();
void LEDControl();
void getData();
void controlAirconditioner(boolean);
void sendToMaster();
void receiveFromMaster(int);

void setup() {
	pinMode(LEDB, OUTPUT);  // 3색 LED중 파란색 핀을 출력핀으로 설정
	pinMode(LEDR, OUTPUT);  // 3색 LED중 붉은색 핀을 출력핀으로 설정
	pinMode(13,OUTPUT);
	Power = false;
	Serial.begin(9600);
	Wire.begin(SLAVE);
	Wire.onRequest(sendToMaster);
	Wire.onReceive(receiveFromMaster);
}

void loop() {
	getData();
	LEDControl();
	if(temp > 30 && !Power)
		controlAirconditioner(true);
	else if(temp < 26 && Power)
		controlAirconditioner(false);
	delay(1000);
}

/*void receiveFromMaster(int bytes) {
	controlAirconditioner(Wire.read());
}*/

void sendToMaster(){
	Wire.write((byte)temp);
	Wire.write((byte)humi);
	Wire.write((byte)discomfort);
	Wire.write((byte)Power);
}

void controlAirconditioner(boolean P){
	if(P != Power){
		if(P)
			for(int i=0; i <=10; i++){
				irsend.sendLG(0x8800909,28);                 // 에어컨 Turn on
				delay(300);                                  // 에어컨 off 신호 발신오류 대비해 0.3초 간격으로 10번 신호발송
				Serial.println("1");
			}
		else
			for(int i=0; i <=10; i++){
				irsend.sendLG(0x88C0051,28);                 // 에어컨 Turn off
				delay(300);                                  // 에어컨 off 신호 발신오류 대비해 0.3초 간격으로 10번 신호발송
				Serial.println("0");
			}

		Power = P;
	}
}

void getData(){
	if(!dht11.read(humi,temp)){
		delay(150);
		blink();
	}
}

void LEDControl(){
	discomfort = (1.8 * temp) - (0.55 * (1 - humi / 100.0) * (1.8 * temp - 26)) + 32;
	int LEDLevel = (discomfort - 75) * 17;
	if(LEDLevel < 0){
		analogWrite(LEDB,255);
		analogWrite(LEDR,0);
	}
	else if(LEDLevel > 255){
		analogWrite(LEDB,0);
		analogWrite(LEDR,255);
	}else{
		analogWrite(LEDB,255 - LEDLevel);
		analogWrite(LEDR,LEDLevel);	
	}
}

void blink(){
	digitalWrite(13, HIGH);
	delay(50);
	digitalWrite(13, LOW);
	delay(50);
}