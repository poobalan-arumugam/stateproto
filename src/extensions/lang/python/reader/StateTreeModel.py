from .parseStateProtoFile import *

class StateTreeNode(Base):
    def __init__(self, parentNode, state):
        self._parentNode = parentNode
        self._state = state
        self._childNodes = []

    def parentNode(self):
        return self._parentNode

    def state(self):
        return self._state

    def name(self):
        return self._state.name

    def addChild(self, child):
        childTreeNode = StateTreeNode(self, child)
        self._childNodes.append(childTreeNode)
        return childTreeNode

    def has_children(self):
        return len(self._childNodes) > 0

    def findNodeForState(self, guid):
        def finder(node, guid):
            if node.state().guid == guid:
                return node
            for child in node._childNodes:
                value = finder(child, guid)
                if value:
                    return value
            return None

        return finder(self, guid)

    def do(self, action, arg):
        for stateNode in self._childNodes:
            action(stateNode, arg)

    def addChildrenFromList(self, stateList):
        for state in stateList:
            if state.parent == self._state.guid:
                childTreeNode = self.addChild(state)
                childTreeNode.addChildrenFromList(stateList)

def buildStateTree(parsedModel):
    class CollectStates:
        def __init__(self, collector):
            self._Collector = collector
            
        def visitState(self, item, arg):
            self._Collector.append(item)
            
        def visitTransition(self, item, arg):
            pass

        def visitStateTransitionPort(self, item, arg):
            pass
        
    states = []
    collector = CollectStates(states)
    parsedModel.do(lambda item, arg: item.accept(collector, arg), None)
    stateTreeRoot = StateTreeNode(None, parsedModel.NoParentState())
    stateTreeRoot.addChildrenFromList(states)
    return stateTreeRoot

