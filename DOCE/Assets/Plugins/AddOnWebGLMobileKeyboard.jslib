mergeInto(LibraryManager.library, {


CheckIfMobile: function(){
  if(UnityLoader.SystemInfo.mobile){
    SendMessage('KeyboardOpener', 'IsMobile');
    console.log("indeed is mobile");
  }
},




OpenInputKeyboard: function(currentValue){
  var string = currentValue;
  if(string == null){
    string = "";
  }

  document.getElementById("fixedInput").value = Pointer_stringify(currentValue);
  document.getElementById("fixedInput").focus();

  console.log("OpenInputKeyboard Initialized");
  SendMessage('InputFieldAddOn', 'IsMobile');
},

CloseInputKeyboard: function(){
  document.getElementById("fixedInput").blur();
  console.log("CloseInputKeyboard Initialized");
},
FixInputOnBlur: function(){
  SendMessage('InputFieldAddOn', 'LoseFocus');
  console.log("FixInputOnBlur Initialized");
},
FixInputUpdate: function (){
  SendMessage('InputFieldAddOn','ReceiveInputChange', document.getElementById("fixedInput").value);
  console.log("FixInputUpdate Initialized");
},
});
