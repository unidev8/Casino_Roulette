mergeInto(LibraryManager.library, {
  isAndroid : function() {
    return Module.SystemInfo.os === "Android";
  },
  getWindowWidth : function() {
    return window.innerWidth;
  },
  getWindowHeight : function() {
    return window.innerHeight;
  },
  isIos : function() {
    return Module.SystemInfo.os === "iPhoneOS" || Module.SystemInfo.os === "iPadOS"; // || Module.SystemInfo.os === "macOS"
  },
  getSearchParams: function () {
        var returnStr = window.location.search;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
  }
});