#TestHsm
from qhsm import *

class TestHsm (QHsm):
    def __init__(self):
        QHsm.__init__(self)
        pass
    def initialiseStateMachine(self):
        self.initialiseState(self.stateX)
        print "initialise"
        pass
    def stateX(self, ev):
        if ev.QSignal == QSignals.Init:
            print "stateX::", ev
            self.initialiseState(self.state0)
            return None
        print "otherwise stateX", ev
        return self._TopState
                                
    def state0(self, ev):
        if ev.QSignal == "Hello":
            print "state0::", ev
            self.transitionTo(self.state1)
            return None
        print "otherwise state0", ev
        return self.stateX
    
    def state1(self, ev):
        if ev.QSignal == "Hello":
            print "state1::", ev
            self.transitionTo(self.state0)
            return None
        print "otherwise state1", ev
        return self._TopState

def printCurrentStateName(hsm):
    print "Get currentStateName"
    currentStateName = hsm.currentStateName()
    print "CurrentStateName:", currentStateName    

def test():
    t = TestHsm()
    t.init()
    printCurrentStateName(t)
    t.dispatch(QEvent("Hello"))
    printCurrentStateName(t)
    t.dispatch(QEvent("Hello"))
    printCurrentStateName(t)


if __name__ == "__main__":
    test()