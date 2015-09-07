// -- debug

function assert(test, msg){
  if(test == false){
    throw msg;
  }
}

// -- define default QSignals

function QSignalsCtor(){
}

// some bug in old netscape
new QSignalsCtor();

QSignalsCtor.prototype.Entry = "QENTRY";
QSignalsCtor.prototype.Exit = "QEXIT";
QSignalsCtor.prototype.Init = "QINIT";
QSignalsCtor.prototype.Empty = "QEMPTY";

var QSignals = new QSignalsCtor();

// -- define qevent

function QEvent(source, signal, data){
  this.QSource = source;
  this.QSignal = signal;
  this.QData = data;
}

function QEventFromSignal(signal){
  return new QEvent(null, signal, null);
}
function QEventFromSignalAndData(signal, data){
  return new QEvent(null, signal, data);
}
function QEventFromSourceAndSignalAndData(source, signal, data){
  return new QEvent(source, signal, data);
}

new QEvent(null, null, null);

// -- define default QSignalsEvents

function QSignalsEventsCtor (){
};

new QSignalsEventsCtor();

QSignalsEventsCtor.prototype.Entry = QEventFromSignal(QSignals.Entry);
QSignalsEventsCtor.prototype.Exit = QEventFromSignal(QSignals.Exit);
QSignalsEventsCtor.prototype.Init = QEventFromSignal(QSignals.Init);
QSignalsEventsCtor.prototype.Empty = QEventFromSignal(QSignals.Empty);

var QSignalsEvents = new QSignalsEventsCtor();

// -- define qhsm

function QHsm(){
  this._CurrentState = this._TopState;
}

// for some bug in old navigator
new QHsm();

QHsm.prototype.initialiseStateMachine = function(){
  throw "override initialise and call initialiseStateMachine(currentState);";
}

QHsm.prototype.initialiseState = function(state){
  this._CurrentState = state;
}

QHsm.prototype.init = function(){
  assert(this._CurrentState == this._TopState);
  
  this.initialiseStateMachine();
  
  var state = this._CurrentState;
  this.trigger(state, QSignalsEvents.Entry);
  this.triggerInitState(state);
}

QHsm.prototype._TopState = function(ev){
  return null;
}

QHsm.prototype.unhandledTransition = function(ev){
  throw "Unhandled Transition: " + ev;
}

QHsm.prototype.trigger = function(state, signalEvent){
  if(null == state || undefined == state){
    throw "Unexpected: " + state + " on signal: " + signal;
  }
  var result = state.call(this, signalEvent);
  return result;
}

QHsm.prototype.dispatch = function(ev){
  this._MySourceState = this._CurrentState;
  while(this._MySourceState != null){
    if(this._MySourceState == this._TopState){
      this.unhandledTransition(ev);
    }
      
    var parentState = this._MySourceState(ev);
    if(parentState != null){
      this._MySourceState = parentState;
    } else {
      this._MySourceState = null;
    }
  }
}

QHsm.prototype.transitionTo = function(targetState){
  assert(targetState != this._TopState);
  this.exitUpToSourceState();
  this.transitionFromSourceStateToTargetState(targetState);
}

QHsm.prototype.getSuperState = function(state){
  var result = this.trigger(state, QSignalsEvents.Empty);
  if(null == result || undefined == result){
    //throw "Unexpected: getSuperState result from state: " + state + " on signal: " + QSignals.Empty + " is null or undefined. result => " + result;
  }
  return result;
}

QHsm.prototype.exitUpToSourceState = function(){
  var state = this._CurrentState;
  while(state != this._MySourceState){
    exitHandler = this.trigger(state, QSignalsEvents.Exit);
    if(exitHandler != null){
      state = exitHandler;
    } else {
      state = this.getSuperState(state);
    }
  }
}

QHsm.prototype.currentStateName = function(){
  if(null == this._CurrentState){
    return "QNULLSTATE";
  }
  var nameList = [];
  var state = this._CurrentState;
  while(state != this._TopState){
    var name = this.getNameOfStateMethod(state);
    nameList.push(name);
    state = this.getSuperState(state);
  }
  var name = "";
  for (var index = nameList.length - 1; index >= 0; index--){
    if (name.length > 0){
        name = name + "." + nameList[index];
    } else {
        name = nameList[index];
    }
  }
  return name;
}

QHsm.prototype.transitionFromSourceStateToTargetState = function(targetState){
  function QHolder(){
  }
  var holder = new QHolder();
  this.exitUpToLCA(targetState, holder);
  this.transitionDownToTargetState(targetState, 
				   holder.statesTargetToLCA,
				   holder.indexFirstStateToEnter);
}

QHsm.prototype.exitUpToLCA = function(targetState, holder){
  holder.statesTargetToLCA = new Array();
  holder.statesTargetToLCA.push(targetState);
  holder.indexFirstStateToEnter = 0;
  
  
  // (a) check my source state == target state (transition to self)
  if(this._MySourceState == targetState){
    this.trigger(this._MySourceState, QSignalsEvents.Exit);
    return;
  }

  // (b) check my source state == super state of the target state
  var targetSuperState = this.getSuperState(targetState);
  if(this._MySourceState == targetSuperState){
    return;
  }

  // (c) check super state of my source state == super state of target state
  // (most common)
  var sourceSuperState = this.getSuperState(this._MySourceState);
  if(sourceSuperState == targetSuperState){
    this.trigger(this._MySourceState, QSignalsEvents.Exit);
    return;
  }
			
  // (d) check super state of my source state == target
  if (sourceSuperState == targetState){
    this.trigger(this._MySourceState, QSignalsEvents.Exit);
    holder.indexFirstStateToEnter = -1; // we don't enter the LCA
    return;
  }
			
  // (e) check rest of my source = super state of super state ... of target state hierarchy
  holder.statesTargetToLCA.push(targetSuperState);
  holder.indexFirstStateToEnter++;
  for (var state = this.getSuperState(targetSuperState);
       state != null; state = this.getSuperState(state)){
    if (this._MySourceState == state)
      {
	return;
      }
				
    holder.statesTargetToLCA.push(state);
    holder.indexFirstStateToEnter++;
  }
			
  // For both remaining cases we need to exit the source state
  this.trigger(this._MySourceState, QSignalsEvents.Exit);
			
  // (f) check rest of super state of my source state ==
  //     super state of super state of ... target state
  // The array list is currently filled with all the states
  // from the target state up to the top state
  for (var stateIndex = holder.indexFirstStateToEnter; stateIndex >= 0; stateIndex--){
    if (sourceSuperState == holder.statesTargetToLCA[stateIndex]){
      holder.indexFirstStateToEnter = stateIndex - 1;
      // Note that we do not include the LCA state itself;
      // i.e., we do not enter the LCA
      return;
    }
  }
			
  // (g) check each super state of super state ... of my source state ==
  //     super state of super state of ... target state
  for (var state = sourceSuperState;
       state != null; 
       state = this.getSuperState(state)){
      for (var stateIndex = holder.indexFirstStateToEnter; stateIndex >= 0; stateIndex--){
        if (state == holder.statesTargetToLCA[stateIndex]){
          holder.indexFirstStateToEnter = stateIndex - 1;
          // Note that we do not include the LCA state itself;
          // i.e., we do not enter the LCA
          return;
        }
      }
      this.trigger(state, QSignalsEvents.Exit);
  }
			
  // We should never get here
  throw new ApplicationException("Mal formed Hierarchical State Machine");
}

QHsm.prototype.transitionDownToTargetState = function(targetState, 
						      statesTargetToLCA,
						      indexFirstStateToEnter){

  // we enter the states in the passed in array in reverse order
  for (var stateIndex = indexFirstStateToEnter; stateIndex >= 0; stateIndex--){
    this.trigger(statesTargetToLCA[stateIndex], QSignalsEvents.Entry);
  }
  
  this._CurrentState = targetState;
			
  // At last we are ready to initialize the target state.
  // If the specified target state handles init then the effective
  // target state is deeper than the target state specified in
  // the transition.
  this.triggerInitState(targetState);
}

QHsm.prototype.triggerInitState = function(state){
  while (null == this.trigger(state, QSignalsEvents.Init)){
    // Initial transition must be one level deep
    assert(state == this.getSuperState(this._CurrentState), "Transition more than one level deep");
    state = this._CurrentState;
    this.trigger(state, QSignalsEvents.Entry);
  }
}

