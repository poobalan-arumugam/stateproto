function writeOut(sv){
    var log = document.getElementById('Log');
    if(log == null){
        var body = document.getElementsByTagName('body')[0]
        var divElement = document.createElement('div');
        divElement.id = 'log';
        body.appendChild(divElement);
        log = divElement;
    }
    log.innerHTML = log.innerHTML + "<div>" + sv + "<div/>";
}

var printCurrentStateName = function(hsm){
    writeOut("Get CurrentStateName");
    var currentStateName = hsm.currentStateName();
    writeOut("CurrentStateName: " + currentStateName);
}

var test = function(){
  writeOut("*** Starting *** " + new Date());
  var q = new TestHsm1();

  q.init();
  printCurrentStateName(q);
  q.dispatch(new QEvent("Hello"));
  printCurrentStateName(q);
  q.dispatch(new QEvent("Hello"));
  printCurrentStateName(q);
}

onload = test;
