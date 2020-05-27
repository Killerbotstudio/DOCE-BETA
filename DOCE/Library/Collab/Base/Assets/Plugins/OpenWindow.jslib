var OpenWindowPlugin = {
    openWindow: function(link)
    {
    	var url = Pointer_stringify(link);


      // if(document.getElementById("overlay") != null){
      //     document.getElementById("overlay").style.display = "block";
      //
      // }
      //



      if(UnityLoader.SystemInfo.mobile){
        document.touchstart = function(){
          window.open(url);
          document.touchstart = null;
        }
       }else {
         document.onmouseup = function()
         {
         	window.open(url);
         	document.onmouseup = null;
          }

       }

  //  }

    // isMobile: function(){
    //   if(UnityLoader.SystemInfo.mobile){
    //     return true;
    //   }else {
    //     return false;
    //   }
     }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);
