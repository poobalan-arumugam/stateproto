from reader.parseStateProtoFile import *

class CodeGenVisitor (GenericPrintVisitor):
    def __init__(self, parsedModel):
        GenericPrintVisitor.__init__(self, parsedModel)

    def getAction(self, action):
        if action == None:
            action = "# no action"
        return action

    def fixParentStateNameIfTopState(self, parentStateName):
        if parentStateName == "TOPSTATE":
            return "_TopState"
        return parentStateName

    def isTopState(self, parentStateName):
        return parentStateName == "_TopState"
    
    def visitState(self, item, arg):
        stateName = item.name
        entryAction = self.getAction(item.entry)
        exitAction = self.getAction(item.exit)
        parentStateName = self._parsedModel.ByGuid(item.parent).name
        parentStateName = self.fixParentStateNameIfTopState(parentStateName)
        parentStateName = self.fixParentStateName(parentStateName)
        transitions = self.getTransitionsTextForState(item)
        childStartStateName = item.childStartStateName
        print self.stateMethodTemplate(locals())
    pass

    def getTransitionsTextForState(self, state):
        def getTransitionToStringIgnoringGuards(transitionGroup):
            transitionEvent = transitionGroup.firstTransition.event
            innerGuardText = self.transitionsToStringWithGuardsTemplate(transitionGroup)
            result = self.transitionsForOuterStateMethodTemplate(locals())
            return result
        
        def reduceGroupedTransition(prevValue, transitionGroup, arg):
            outerLevel = getTransitionToStringIgnoringGuards(transitionGroup)
            result = prevValue + outerLevel
            return result

        sv = state.groupedTransitionList.reduce("", reduceGroupedTransition, None)
        return sv
    pass

pass
