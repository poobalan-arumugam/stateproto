TestSample1.prototype.enterStateX = function (){
    writeOut("Enter StateX");
}
TestSample1.prototype.exitStateX = function (){
    writeOut("Exit StateX");
}

TestSample1.prototype.enterState0 = function (){
    writeOut("Enter State0");
}
TestSample1.prototype.exitState0 = function (){
    writeOut("Exit State0");
}

TestSample1.prototype.enterState1 = function (){
    writeOut("Enter State1");
}
TestSample1.prototype.exitState1 = function (){
    writeOut("Exit State1");
}

TestSample1.prototype.sayHello1 = function (){
    writeOut("Hello1");
}
TestSample1.prototype.sayHello2 = function (){
    writeOut("Hello2");
}

TestSample1.prototype.Ok = function (ev){
    return ev.QData;
}
TestSample1.prototype.sayHello3 = function (){
    writeOut("Hello3");
}

// ------------

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
  var q = new TestSample1();

  q.init();
  printCurrentStateName(q);
  q.dispatch(QEventFromSignalAndData("Hello", true));
  q.dispatch(QEventFromSignal("Hello"));
  printCurrentStateName(q);
  q.dispatch(QEventFromSignal("Hello"));
  q.dispatch(QEventFromSignalAndData("Hello", true));
  printCurrentStateName(q);
}

onload = test;
