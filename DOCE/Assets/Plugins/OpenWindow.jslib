 mergeInto(LibraryManager.library, {
//var OpenWindowPlugin = {
  openWindow: function(link){
    var url = Pointer_stringify(link);
    if(UnityLoader.SystemInfo.mobile){
      console.log("is mobile");

      document.ontouchend = function(){
        console.log("on touch end");
        window.open(url);
        document.ontouchend = null;
      }
    }else{
      document.onmouseup = function(){
        console.log("on mouse up");
        window.open(url);
        document.onmouseup = null;
      }
    }
  },

  onGameStartJS: function(){
    console.log("##Start");
  },
});



// });
// var OpenWindowPlugin = {
//     openWindow: function(link)
//     {
//     	var url = Pointer_stringify(link);
//
//       if(UnityLoader.SystemInfo.mobile){
//         console.log ("is mobile");
//
//
//         document.ontouchend = function(){
//           console.log ("on touch end");
//           window.open(url);
//           document.ontouchend = null;
//         }
//
//        }else {
//
//          document.onmouseup = function()
//          {
//            console.log ("mouse up");
//          	window.open(url);
//          	document.onmouseup = null;
//           }
//         }
//      }
// };
//
// onGameStartJS: function(){
//   console.log("##Start");
// }
//
 //mergeInto(LibraryManager.library, OpenWindowPlugin);
