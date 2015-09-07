from qhsm import QEventFromSignalAndData, QEventFromSignal
import testsamplehsm1

class TestSample1Impl(testsamplehsm1.TestSample1):
    def enterStateX(self):
        print "enterStateX"
        pass
    def exitStateX(self):
        print "exitStateX"
        pass
    def enterState0(self):
        print "enterState0"
        pass
    def exitState0(self):
        print "exitState0"
        pass
    def enterState1(self):
        print "enterState1"
        pass
    def exitState1(self):
        print "exitState1"
        pass
    def sayHello1(self):
        print "Hello1"
    def sayHello2(self):
        print "Hello2"
    def sayHello3(self):
        print "Hello3"
    def Ok(self, ev):
        return ev.QData
    

def printCurrentStateName(hsm):
    print "Get currentStateName"
    currentStateName = hsm.currentStateName()
    print "CurrentStateName:", currentStateName    

def test():
    t = TestSample1Impl()
    t.init()
    printCurrentStateName(t)
    t.dispatch(QEventFromSignalAndData("Hello", True))
    t.dispatch(QEventFromSignal("Hello"))
    printCurrentStateName(t)
    t.dispatch(QEventFromSignal("Hello"))
    t.dispatch(QEventFromSignalAndData("Hello", True))
    printCurrentStateName(t)


if __name__ == "__main__":
    test()