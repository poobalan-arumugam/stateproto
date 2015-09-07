# -- define default QSignals

def assert(assertion, msg)
    if(!assertion) then
        throw msg
    end
end

def assert1(assertion)
    if(!assertion) then
        throw "assertion"
    end
end

class QSignals
    Entry = "QENTRY"
    Exit = "QEXIT"
    Init = "QINIT"
    Empty = "QEMPTY"
end

# -- define qevent

class QEvent   
    attr_accessor :qsource, :qsignal, :qdata
    
    def initialize(source, signal, data)
        @qsource = source
        @qsignal = signal
        @qdata = data
    end

    def to_s()
        return "#{qsource}.#{qsignal}.#{qdata}"
    end
end


def qeventFromSignal(signal)
    return QEvent.new(nil, signal, nil)
end

def qeventFromSignalAndData(signal, data)
    return QEvent.new(nil, signal, data)
end

def qeventFromSourceAndSignalAndData(source, signal, data)
    return QEvent.new(source, signal, data)
end

# -- define QSignalsEvents

class QSignalsEvents
    Entry = qeventFromSignal(QSignals::Entry)
    Exit = qeventFromSignal(QSignals::Exit)
    Init = qeventFromSignal(QSignals::Init)
    Empty = qeventFromSignal(QSignals::Empty)
end

# -- define qhsm

class QHsm
    def initialize()
        @_topState = method(:_TopState)
        @_currentState = method(:_TopState)
    end

    def initialiseStateMachine()
        throw "override initialise and call initialiseStateMachine(currentState)"
    end

    def initialiseState(state)
        @_currentState = state
    end

    def init()
        assert1 @_currentState == method(:_TopState)
        
        initialiseStateMachine()
        
        state = @_currentState
        trigger(state, QSignalsEvents::Entry)
        triggerInitState(state)
    end

    def _TopState(ev)
        return nil
    end

    def unhandledTransition(ev)
        throw "Unhandled Transition: " + ev.to_s()
    end

    def trigger(state, signalEvent)
        if (nil == state) then
            throw "Unexpected: " + str(state) + " on signal: " + str(signal)
        end
        
        result = state.call(signalEvent)
        return result
    end

    def dispatch(ev)
        @_mySourceState = @_currentState
        while (@_mySourceState != nil)
            if (@_mySourceState == @_topState) then
                unhandledTransition(ev)
            end
            
            state = @_mySourceState
            parentState = state.call(ev)
            if (parentState != nil) then
                @_mySourceState = parentState            
            else
                @_mySourceState = nil
            end
        end
    end
    
    def transitionTo(targetState)
        assert(targetState != @_topState, "TargetState #{targetState} in TransitionTo is TopState")
        exitUpToSourceState()
        transitionFromSourceStateToTargetState(targetState)
    end

    def getSuperState(state)
        result = trigger(state, QSignalsEvents::Empty)
        if (nil == result) then
            #throw "Unexpected: getSuperState result from state: " + str(state) + " on signal: " + str(QSignalsEvents.Empty) + " is none or undefined. result => " + str(result)
        end
        return result
    end

    def exitUpToSourceState()
        state = @_currentState
        while (state != @_mySourceState)
            exitHandler = trigger(state, QSignalsEvents::Exit)
            if (exitHandler != nil) then
                state = exitHandler
            else
                state = getSuperState(state)
            end
        end
    end

    def currentStateName()
        if (nil == @_currentState) then
            return "QNULLSTATE"
        end
        
        state = @_currentState
        name = []
        while (state != @_topState)
            name.insert(0, state.to_s())
            state = getSuperState(state)
        end
        name = name.join(".")
        return name
    end
    
    class Holder
        attr_accessor :statesTargetToLCA, :indexFirstStateToEnter
    end
    
    def transitionFromSourceStateToTargetState(targetState)
        holder = Holder.new()
        exitUpToLCA(targetState, holder)
        transitionDownToTargetState(targetState,
            holder.statesTargetToLCA,
            holder.indexFirstStateToEnter)
    end
    
    def exitUpToLCA(targetState, holder)
        holder.statesTargetToLCA = []
        holder.statesTargetToLCA.push(targetState)
        holder.indexFirstStateToEnter = 0
        
        # (a) check my source state == target state (transition to self)
        if (@_mySourceState == targetState) then
            trigger(@_mySourceState, QSignalsEvents::Exit)
            return
        end
        
        # (b) check my source state == super state of the target state
        targetSuperState = getSuperState(targetState)
        if (@_mySourceState == targetSuperState) then
            return
        end
        
        # (c) check super state of my source state == super state of target state
        # (most common)
        sourceSuperState = getSuperState(@_mySourceState)
        if (sourceSuperState == targetSuperState) then
            trigger(@_mySourceState, QSignalsEvents::Exit)
            return
        end
        
        # (d) check super state of my source state == target
        if (sourceSuperState == targetState) then
            trigger(@_mySourceState, QSignalsEvents::Exit)
            holder.indexFirstStateToEnter = -1 # we don't enter the LCA
            return
        end
        
        # (e) check rest of my source = super state of super state ... of target state hierarchy
        holder.statesTargetToLCA.push(targetSuperState)
        holder.indexFirstStateToEnter += 1
        state = getSuperState(targetSuperState)
        while (state != nil)
            if (@_mySourceState == state) then
                return
            end
            
            holder.statesTargetToLCA.push(state)
            holder.indexFirstStateToEnter += 1
            state = getSuperState(state)
        end
        
        # For both remaining cases we need to exit the source state
        trigger(@_mySourceState, QSignalsEvents::Exit)
        
        # (f) check rest of super state of my source state ==
        #     super state of super state of ... target state
        # The array list is currently filled with all the states
        # from the target state up to the top state
        holder.indexFirstStateToEnter.downto(0) do |stateIndex|
            if (sourceSuperState == holder.statesTargetToLCA[stateIndex]) then
                holder.indexFirstStateToEnter = stateIndex - 1
                # Note that we do not include the LCA state itself
                # i.e., we do not enter the LCA
                return
            end
        end
        
        # (g) check each super state of super state ... of my source state ==
        #     super state of super state of ... target state
        state = sourceSuperState
        while (state != nil)
            holder.indexFirstStateToEnter.downto(0) do |stateIndex|
                if (state == holder.statesTargetToLCA[stateIndex]) then
                    holder.indexFirstStateToEnter = stateIndex - 1
                    # Note that we do not include the LCA state itself
                    # i.e., we do not enter the LCA
                    return
                end
            end
            
            trigger(state, QSignalsEvents::Exit)
            state = getSuperState(state)
        end
        
        # We should never get here
        throw "Mal formed Hierarchical State Machine"
    end
    
    def transitionDownToTargetState(
        targetState,
        statesTargetToLCA,
        indexFirstStateToEnter)

        # we enter the states in the passed in array in reverse order
        indexFirstStateToEnter.downto(0) do |stateIndex|
            trigger(statesTargetToLCA[stateIndex], QSignalsEvents::Entry)
        end
        
        @_currentState = targetState
        # At last we are ready to initialize the target state.
        # If the specified target state handles init then the effective
        # target state is deeper than the target state specified in
        # the transition.
        triggerInitState(targetState)
    end

    def triggerInitState(state)
        while (nil == trigger(state, QSignalsEvents::Init))
            # Initial transition must be one level deep
            assert1 state == getSuperState(@_currentState)
            state = @_currentState
            trigger(state, QSignalsEvents::Entry)
        end
    end
    
end # end class QHsm


