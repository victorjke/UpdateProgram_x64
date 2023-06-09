//+------------------------------------------------------------------+
//|                                                     TestMQL5.mq5 |
//|                                  Copyright 2023, MetaQuotes Ltd. |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright 2021, DJ Ermoloff"
#property link      "https://tlgg.ru/@djermoloff"
#property version   "1.00"
#property strict

#import "UpdateProgram.dll"
   string GetActualizedProgramInfoJson(string requestUrl);
   bool UpdateProgram(string fileDownloadingUrl);
   void RestartTerminal();
   bool CheckRDPConnection();
   void TerminalHide();
   void TerminalShow();
#import

#include <JAson.mqh>

string url_version = "https://mt-api.darkband.ru/get-ea-version.php?project=test";
CJAVal json;
string file_url;
bool success;

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//---
   string name = "get-act";
   ObjectCreate(0,name,OBJ_BUTTON,0,50,50);
   ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_LEFT_LOWER);
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,5);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,25);
   ObjectSetInteger(0,name,OBJPROP_XSIZE,150);
   ObjectSetInteger(0,name,OBJPROP_YSIZE,20);
   ObjectSetString(0,name,OBJPROP_TEXT,"Получить ссылку");
   ObjectSetInteger(0,name,OBJPROP_STATE,0);
   
   name = "update";
   ObjectCreate(0,name,OBJ_BUTTON,0,50,50);
   ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_LEFT_LOWER);
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,5);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,45);
   ObjectSetInteger(0,name,OBJPROP_XSIZE,150);
   ObjectSetInteger(0,name,OBJPROP_YSIZE,20);
   ObjectSetString(0,name,OBJPROP_TEXT,"Обновить робота");
   ObjectSetInteger(0,name,OBJPROP_STATE,0);
//---
   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//---
   ObjectDelete(0,"get-act");
   ObjectDelete(0,"update");
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
//---
   
  }
//+------------------------------------------------------------------+

void OnChartEvent(const int id,const long &lparam,const double &dparam,const string &sparam) {
   if(id==CHARTEVENT_OBJECT_CLICK) {
      if (sparam == "get-act") {
         Print("Нажата кнопка \"Получить робота\"");
         string version_info_json = GetActualizedProgramInfoJson(url_version);
         Print(version_info_json);
         ObjectSetInteger(0,"get-act",OBJPROP_STATE,0);
         json.Deserialize(version_info_json);
         file_url = json["url"].ToStr();
         /*Print(CheckRDPConnection());
         TerminalHide();
         Sleep(5000);
         TerminalShow();*/
      }
      if (sparam == "update") {
         Print("Нажата кнопка \"Обновить\"");         
         ObjectSetInteger(0,"update",OBJPROP_STATE,0);
         success = UpdateProgram::UpdateProgram(file_url);
         if (success)
            RestartTerminal();
      }
   }
}