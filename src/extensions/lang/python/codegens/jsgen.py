from reader.parseStateProtoFile import *
from codegenbase import *
import string

class JavascriptVisitor (CodeGenVisitor):
    def __init__(self, parsedModel):
        CodeGenVisitor.__init__(self, parsedModel)

    def getAction(self, action):
        if action == None:
            action = ""
        else:
            actions = action.split(";")
            action = ["this." + action  for action in actions]
            action = string.join(action, """;
        """)
        return action

    def fixParentStateName(self, parentStateName):
        if not self.isTopState(parentStateName):
            return "s_" + parentStateName
        return parentStateName    

    def stateMethodTemplate(self, dict):
        dict['statemachine'] = self._parsedModel.header().statemachine
        startStatePattern = ""
        if dict['childStartStateName'] != None:
            startStatePattern = """ else if (ev.QSignal == QSignals.Init){
        this.initialiseState(this.s_%(childStartStateName)s);
    }""" % dict
            pass
        dict['startStatePattern'] = startStatePattern
        return """
%(statemachine)s.prototype.s_%(stateName)s = function(ev){
    %(transitions)sif (ev.QSignal == QSignals.Entry) {
        %(entryAction)s;
    } else if (ev.QSignal == QSignals.Exit) {
        %(exitAction)s;
    }%(startStatePattern)s else {
        return this.%(parentStateName)s;
    }
    return null;
}""" % dict


    def transitionsForOuterStateMethodTemplate(self, dict):
        return """if (ev.QSignal == "%(transitionEvent)s") {
        %(innerGuardText)s
    } else """ % dict
    pass

    def transitionsToStringWithGuardsTemplate(self, transitionGroup):
        def guardedTransToText(transition, isFirstTransition):
            guard = transition.guard
            transitionAction = self.getAction(transition.action)
            toStateName = self._parsedModel.ByGuid(transition.tostate).name
            if guard:
                elExpr = ""
                if not isFirstTransition:
                    elExpr = " else "
                return elExpr + """if (this.%(guard)s) {
            %(transitionAction)s;
            this.transitionTo(this.s_%(toStateName)s);
        }""" % locals()
            else:
                if isFirstTransition:
                    return """%(transitionAction)s;
        this.transitionTo(this.s_%(toStateName)s);""" % locals()
                else:
                    return """ else {
            %(transitionAction)s;
            this.transitionTo(this.s_%(toStateName)s);
        }""" % locals()
                

        def reducePerCondition(prevValue, transition, arg):
            return prevValue + guardedTransToText(transition, prevValue == "")
            
        result = transitionGroup.reduce("", reducePerCondition, None)
        return result        
    pass

    #end of class JavascriptVisitor
    pass

class JavascriptLanguageGenerator(object):
    def generatorVersion(self):
        return "0.1"
    def setModel(self, model):
        self._Model = model
        self._StateMethodGenerator = JavascriptVisitor(model)
        pass
    def writeClassHeader(self, header):
        statemachine = header.statemachine
        defaultStartState = self._Model.defaultStartState.name
        generatorVersion = self.generatorVersion()
        print """
// generated by JavascriptGenerator version %(generatorVersion)s

function %(statemachine)s(){
}

// dummy for prototype for netscape
new %(statemachine)s();

%(statemachine)s.prototype = new QHsm();

%(statemachine)s.prototype.initialiseStateMachine = function(){
    this.initialiseState(this.s_%(defaultStartState)s)
};
""" % locals()
        pass
    def writeClassTrailer(self, header):
        statemachine = header.statemachine

        def composeStateMethodTests(state, composer):
            statename = state.name
            sv = ""
            if len(composer) > 0:
                sv = """
    """
                pass
            sv += """if(stateMethod == this.s_%(statename)s){
        return "%(statename)s";
    }""" % locals()
            composer.append(sv)
            pass

        composer = []            
        self._Model.stateList().do(composeStateMethodTests, composer)
        import string
        stateMethodTests = string.join(composer, "")

        print """
%(statemachine)s.prototype.getNameOfStateMethod = function (stateMethod){
    %(stateMethodTests)s
    return QHsm.getNameOfStateMethod.call(this, stateMethod);
}""" % locals()
        
        print """
//end of %(statemachine)s
""" % locals()
        pass
    def writeStartClassBody(self):
        pass
    def writeStateMethod(self, state):
        self._StateMethodGenerator.visitState(state, None)
        pass
    def writeEndClassBody(self):
        pass
    pass

