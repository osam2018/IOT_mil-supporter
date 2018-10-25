
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

String cmd;
void loop() {
  // put your main code here, to run repeatedly:
  if(Serial.available()){
    char c = Serial.read();
    if(c == '@'){
      Serial.println(cmd);
      cmd = "";
    }
    else{
      cmd += c;
    } 
  }
}
