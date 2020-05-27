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
        console.log ("is mobile");


        document.ontouchend = function(){
          console.log ("on touch end");
          window.open("https://www.geeksforgeeks.org");
          document.ontouchend = null;
        }

        // document.ontouchstart = function(){
        //   console.log("on touch start");
        //   window.open(url);
        //   document.ontouchstart = null;
        // }
        //
        // document.onmouseup = function(){
        //   console.log("on mouse up");
        //   window.open("https://www.google.com");
        //   document.onmouseup = null;
        // }
        //
        // function simulateClick(){
        //   console.log("simulate click");
        //   document.querySelector('#btn1').click();
        // }
        // simulateClick();

       }else {

         document.onmouseup = function()
         {
           console.log ("mouse up");
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
