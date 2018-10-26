#include <Wire.h>

#define SLAVE 20

int doorSwitch = 7; 	// 문 버튼의 값을 읽을 포트
int coverSwitch = 4;	// 커버 버튼의 값을 읽을 포트
int cds = A0;			// 커버 조도센서의 값을 읽을 포트
int water = A1;			// 수분감지센서의 값을 읽을 포트

boolean isDoorClosed = false;
boolean isCoverUsing = false;
boolean isCdsUsing = false;
boolean isToiletSeatUsing = false;
boolean isWaterStability = false;

void blink();
void DoorScan();
void CdsScan();
void CoverScan();
void ToiletSeatScan();
void WaterScan();
void ScanManager();
void LCDManager();
void sendToMaster();

void setup(){
	pinMode(doorSwitch,INPUT);	// 문 스위치 입력
	pinMode(coverSwitch,INPUT);	// 문 스위치 입력
	pinMode(13,OUTPUT);			// 문 스위치 입력
	isDoorClosed = false;		// 문 플래그
	isCoverUsing = false;		// 커버 플래그
	isCdsUsing = false;			// 조도 플래그
	isToiletSeatUsing = false;	// 시트 플래그
	isWaterStability = false;	// 수분 플래그
	Wire.begin(SLAVE);
	Wire.onRequest(sendToMaster);
}

void loop(){
	ScanManager();
}

void sendToMaster(){
	Wire.write(isDoorClosed + 2 * isToiletSeatUsing + 4 * isWaterStability);
	blink();
}

void ScanManager(){
	DoorScan();
	CdsScan();
	CoverScan();
	ToiletSeatScan();
	WaterScan();
}

void DoorScan(){
	isDoorClosed = !digitalRead(doorSwitch);
}

void CdsScan(){
	isCdsUsing = analogRead(cds) > 220;
}

void CoverScan(){
	isCoverUsing = !digitalRead(coverSwitch);
}

void ToiletSeatScan(){
	isToiletSeatUsing = isCoverUsing && isCdsUsing;
}

void WaterScan(){
	isWaterStability = analogRead(water) > 50;
}

void blink(){
	digitalWrite(13, HIGH);
	delay(50);
	digitalWrite(13, LOW);
	delay(50);
}
