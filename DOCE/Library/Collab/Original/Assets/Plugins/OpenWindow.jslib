var OpenWindowPlugin = {
    openWindow: function(link)
    {
    	var url = Pointer_stringify(link);


      var a = window.document.createElement("a");
      a.target = '_blank';
      a.href = url;

      var e = window.document.createEvent("MouseEvents");
      e.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
      a.dispatchEvent(e);

  }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);
