// -- tests from here onwards

function TestHsm1(){
};

new TestHsm1();

TestHsm1.prototype = new QHsm();

TestHsm1.prototype.stateX = function(ev){
  if(ev.QSignal == QSignals.Init){
    writeOut("stateX:" + ev.QSignal);
    this.initialiseState(this.state0);
    return null;
  }
    writeOut("otherwise stateX " + ev.QSignal);
  return this._TopState;
};

TestHsm1.prototype.state0 = function(ev){
  if(ev.QSignal == "Hello"){
    writeOut("state0:" + ev.QSignal);
    this.transitionTo(this.state1);
    return null;
  }
    writeOut("otherwise state0 " + ev.QSignal);
  return this.stateX;
};

TestHsm1.prototype.state1 = function(ev){
  if(ev.QSignal == "Hello"){
    writeOut("state1:" + ev.QSignal);
    this.transitionTo(this.state0);
    return null;
  }
    writeOut("otherwise state1 " + ev.QSignal);
  return this._TopState;
};
  
TestHsm1.prototype.initialiseStateMachine = function(){
    writeOut("Initialise");
  this.initialiseState(this.stateX);
};

TestHsm1.prototype.getNameOfStateMethod = function (stateMethod){
    if(stateMethod == this.stateX){
        return "stateX";
    } 
    if(stateMethod == this.state0){
        return "state0";
    } 
    if(stateMethod == this.state1){
        return "state1";
    } 
    return QHsm.getNameOfStateMethod.call(this, stateMethod);
}
