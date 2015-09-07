from reader.parseStateProtoFile import *
from codegenbase import *

class SmalltalkVisitor (CodeGenVisitor):
    def __init__(self, parsedModel):
        CodeGenVisitor.__init__(self, parsedModel)
        
    def stateMethodTemplate(self, dict):
        return """
    %(stateName)s: ev
        %(transitions)s(ev.QSignal == QSIGNAL.C_ENTRY)
            ifTrue:[%(entryAction)s]
            ifFalse:[
                (ev.QSignal == QSIGNAL.C_EXIT)
                    ifTrue: [%(exitAction)s]
                ]
            
        ^ #%(parentStateName)s:.
    }""" % dict
    pass

    def transitionsForStateMethodTemplate(self, dict):
        return """(ev.QSignal = "%(transitionEvent)s")
                ifTrue: [
                    %(transitionAction)s.
                    TransitionTo: #%(toStateName)s:.
                    ]
                ifFalse: [
         """ % dict
    pass
pass
