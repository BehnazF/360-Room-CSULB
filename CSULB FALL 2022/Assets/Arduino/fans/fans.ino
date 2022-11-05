
#define LED_PIN 13
#define BUTTON_PIN 3



void setup(){
  Serial.begin(9600);
  
  pinMode(LED_PIN, OUTPUT);
  pinMode(BUTTON_PIN, INPUT);
  digitalWrite(BUTTON_PIN, HIGH);

}
void loop(){


    if (digitalRead(BUTTON_PIN) == HIGH){
      digitalWrite(LED_PIN,HIGH);
      Serial.write(1);
      Serial.flush();
      delay(20);
    }
    else {
      digitalWrite(LED_PIN,LOW);
      Serial.write(2);
      Serial.flush();
      delay(20);
    }
  }
