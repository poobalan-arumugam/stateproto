require 'qhsm.rb'
require 'testsamplehsm1.rb'

class TestSample1Impl < TestSample1
    def enterStateX()
        puts "enterStateX"
    end
    def exitStateX()
        puts "exitStateX"
    end
    def enterState0()
        puts "enterState0"
    end
    def exitState0()
        puts "exitState0"
    end
    def enterState1()
        puts "enterState1"
    end
    def exitState1()
        puts "exitState1"
    end
    def sayHello1()
        puts "Hello1"
    end
    def sayHello2()
        puts "Hello2"
    end
    def sayHello3()
        puts "Hello3"
    end
    def Ok(ev)
        return ev.qdata
    end
end


    

def printCurrentStateName(hsm)
    puts "Get currentStateName"
    currentStateName = hsm.currentStateName()
    puts "CurrentStateName:", currentStateName    
end

def test()
    t = TestSample1Impl.new()
    t.init()
    printCurrentStateName(t)
    t.dispatch(qeventFromSignalAndData("Hello", true))
    t.dispatch(qeventFromSignal("Hello"))
    printCurrentStateName(t)
    t.dispatch(qeventFromSignal("Hello"))
    t.dispatch(qeventFromSignalAndData("Hello", true))
    printCurrentStateName(t)
end

test()