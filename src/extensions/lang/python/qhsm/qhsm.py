import string

# -- define default QSignals

class QSignals:
    Entry = "QENTRY"
    Exit = "QEXIT"
    Init = "QINIT"
    Empty = "QEMPTY"
pass

# -- define qevent

class QEvent:    
    def __init__(self, source, signal, data):
        self.QSource = source
        self.QSignal = signal
        self.QData = data

    def __str__(self):
        return "%s.%s.%s" % (self.QSource, self.QSignal, self.QData)
pass


def QEventFromSignal(signal):
    return QEvent(None, signal, None)

def QEventFromSignalAndData(signal, data):
    return QEvent(None, signal, data)

def QEventFromSourceAndSignalAndData(source, signal, data):
    return QEvent(source, signal, data)

# -- define QSignalsEvents

class QSignalsEvents:
    Entry = QEventFromSignal(QSignals.Entry)
    Exit = QEventFromSignal(QSignals.Exit)
    Init = QEventFromSignal(QSignals.Init)
    Empty = QEventFromSignal(QSignals.Empty)
pass

# -- define qhsm

class QHsm:
    def __init__(self):
        self._CurrentState = self._TopState

    def initialiseStateMachine(self):
        raise "override initialise and call initialiseStateMachine(currentState)"

    def initialiseState(self, state):
        self._CurrentState = state

    def init(self):
        assert self._CurrentState == self._TopState
        
        self.initialiseStateMachine()
        
        state = self._CurrentState
        self.trigger(state, QSignalsEvents.Entry)
        self.triggerInitState(state)

    def _TopState(self, ev):
        return None

    def unhandledTransition(self, ev):
        raise "Unhandled Transition: " + str(ev)

    def trigger(self, state, signalEvent):
        if None == state:
            raise "Unexpected: " + str(state) + " on signal: " + str(signal)
        pass
        
        result = state(signalEvent)
        return result

    def dispatch(self, ev):
        self._MySourceState = self._CurrentState
        while self._MySourceState != None:
            if self._MySourceState == self._TopState:
                self.unhandledTransition(ev)
            
            parentState = self._MySourceState(ev)
            if parentState != None:
                self._MySourceState = parentState
            else:
                self._MySourceState = None
            pass
        pass
    

    def transitionTo(self, targetState):
        assert targetState != self._TopState, "TargetState {%s} in TransitionTo is TopState" % (targetState,)
        self.exitUpToSourceState()
        self.transitionFromSourceStateToTargetState(targetState)

    def getSuperState(self, state):
        result = self.trigger(state, QSignalsEvents.Empty)
        if None == result:
            #raise "Unexpected: getSuperState result from state: " + str(state) + " on signal: " + str(QSignalsEvents.Empty) + " is none or undefined. result => " + str(result)
            pass
        return result

    def exitUpToSourceState(self):
        state = self._CurrentState
        while state != self._MySourceState:
            exitHandler = self.trigger(state, QSignalsEvents.Exit)
            if exitHandler != None:
                state = exitHandler
            else:
                state = self.getSuperState(state)
            pass
        pass

    def currentStateName(self):
        if None == self._CurrentState:
            return "QNULLSTATE"
        
        state = self._CurrentState
        name = []
        while state != self._TopState:
            name.insert(0, state.__name__)
            state = self.getSuperState(state)
        name = string.join(name, ".")
        return name
    
    def transitionFromSourceStateToTargetState(self, targetState):
        class Holder:
            pass
        holder = Holder()
        self.exitUpToLCA(targetState, holder)
        self.transitionDownToTargetState(targetState,
            holder.statesTargetToLCA,
            holder.indexFirstStateToEnter)
        pass
    
    def exitUpToLCA(self, targetState, holder):
        holder.statesTargetToLCA = []
        holder.statesTargetToLCA.append(targetState)
        holder.indexFirstStateToEnter = 0
        
        # (a) check my source state == target state (transition to self)
        if self._MySourceState == targetState:
            self.trigger(self._MySourceState, QSignalsEvents.Exit)
            return
        
        # (b) check my source state == super state of the target state
        targetSuperState = self.getSuperState(targetState)
        if self._MySourceState == targetSuperState:
            return
        
        # (c) check super state of my source state == super state of target state
        # (most common)
        sourceSuperState = self.getSuperState(self._MySourceState)
        if sourceSuperState == targetSuperState:
            self.trigger(self._MySourceState, QSignalsEvents.Exit)
            return
        
        # (d) check super state of my source state == target
        if sourceSuperState == targetState:
            self.trigger(self._MySourceState, QSignalsEvents.Exit)
            holder.indexFirstStateToEnter = -1 # we don't enter the LCA
            return
        
        # (e) check rest of my source = super state of super state ... of target state hierarchy
        holder.statesTargetToLCA.append(targetSuperState)
        holder.indexFirstStateToEnter += 1
        state = self.getSuperState(targetSuperState)
        while state != None:
            if self._MySourceState == state:
                return
            
            holder.statesTargetToLCA.append(state)
            holder.indexFirstStateToEnter += 1
            state = self.getSuperState(state)
            pass
        
        # For both remaining cases we need to exit the source state
        self.trigger(self._MySourceState, QSignalsEvents.Exit)
        
        # (f) check rest of super state of my source state ==
        #     super state of super state of ... target state
        # The array list is currently filled with all the states
        # from the target state up to the top state
        for stateIndex in xrange(holder.indexFirstStateToEnter, -1, -1):
            if sourceSuperState == holder.statesTargetToLCA[stateIndex]:
                holder.indexFirstStateToEnter = stateIndex - 1
                # Note that we do not include the LCA state itself
                # i.e., we do not enter the LCA
                return
            pass
        
        # (g) check each super state of super state ... of my source state ==
        #     super state of super state of ... target state
        state = sourceSuperState
        while state != None:
            for stateIndex in xrange(holder.indexFirstStateToEnter, -1, -1):
                if (state == holder.statesTargetToLCA[stateIndex]):
                    holder.indexFirstStateToEnter = stateIndex - 1
                    # Note that we do not include the LCA state itself
                    # i.e., we do not enter the LCA
                    return
                pass
            self.trigger(state, QSignalsEvents.Exit)
            state = self.getSuperState(state)
            pass
        
        # We should never get here
        raise "Mal formed Hierarchical State Machine"
        pass
    
    def transitionDownToTargetState(self,
        targetState,
        statesTargetToLCA,
        indexFirstStateToEnter):

        # we enter the states in the passed in array in reverse order
        for stateIndex in xrange(indexFirstStateToEnter, -1, -1):
            self.trigger(statesTargetToLCA[stateIndex], QSignalsEvents.Entry)
            pass
        
        self._CurrentState = targetState
        # At last we are ready to initialize the target state.
        # If the specified target state handles init then the effective
        # target state is deeper than the target state specified in
        # the transition.
        self.triggerInitState(targetState)

    def triggerInitState(self, state):        
        while None == self.trigger(state, QSignalsEvents.Init):
            # Initial transition must be one level deep
            assert state == self.getSuperState(self._CurrentState)
            state = self._CurrentState
            self.trigger(state, QSignalsEvents.Entry)
            pass
        pass
    
    # end class QHsm
    pass

