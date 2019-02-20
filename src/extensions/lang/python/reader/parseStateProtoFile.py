from __future__ import print_function

import io

class PrintStream(object):
    def __init__(self):
        self._io = io.StringIO()
        self._indent = 0
        self.resetIndent()
    def writeItem(self, item):
        self.beginWriteItem()
        try:
            self.writeItemName(item.__class__.__name__)
            self.write("(\n")
            self.beginWriteItem()
            skipAttributesNamed = {}
            if hasattr(item, "_skipAttributesNamed"):
                skipAttributesNamed = item._skipAttributesNamed()
            try:
                for name, value in item.__dict__.items():
                    if not name in skipAttributesNamed:
                        self.writeItemAttribute(name, value)
            finally:
                self.endWriteItem()
                self.write(") // " + item.__class__.__name__ + "\n")
        finally:
            self.endWriteItem()

    def beginWriteItem(self):
        self._indent += 1
        self.notifyIndentationChanged()

    def endWriteItem(self):
        self._indent -= 1
        self.notifyIndentationChanged()

    def writeItemName(self, name):
        self.write(name + "\n")

    def writeList(self, lst):
        self.beginWriteItem()
        counter = 0
        self.write("Count: %s\n" % (len(lst),))
        self.write("[\n");
        self.beginWriteItem()
        for item in lst:
            self.writeItemAttribute("%s: " % (counter,), item)
            counter += 1
        self.endWriteItem()
        self.write("]\n");
        self.endWriteItem()

    def writeItemAttribute(self, name, value):
        if hasattr(value, "_toStream"):
            self.write("%s {\n" % (name,))
            self.beginWriteItem()
            value._toStream(self)
            self.endWriteItem()
            self.write("}; // %s\n" % (name,))
        else:
            self.write("%s = %s;\n" % (name, value,))

    def resetIndent(self):
        self.notifyIndentationChanged()

    def notifyIndentationChanged(self):
        self._indentLine = None

    def indentAsLine(self):
        if self._indentLine == None:
            self._indentLine = " " * (4 * self._indent)
        return self._indentLine

    def writeRaw(self, value):
        self._io.write(value)

    def write(self, value):
        self._io.write(self.indentAsLine())
        self.writeRaw(value)

    def __repr__(self):
        return self._io.getvalue()

class Base(object):
    def __init__(self, dict):
        self.__dict__.update(dict)

    def _makePrintStream(self):
        return PrintStream()

    def _skipAttributesNamed(self):
        return {"self":""}

    def _toStream(self, stream):
        stream.writeItem(self)

    def __repr__(self):
        printStream = self._makePrintStream()
        self._toStream(printStream)
        return repr(printStream)
    pass

class Header(Base):
    def __init__(self, dict):
        self.__dict__.update(dict)
    pass

class State(Base):
    def __init__(self, dict):
        self.__dict__.update(dict)
    pass
    def accept(self, visitor, arg):
        return visitor.visitState(self, arg)
    pass


class Transition(Base):
    def __init__(self, dict):
        self.__dict__.update(dict)
    pass
    def accept(self, visitor, arg):
        return visitor.visitTransition(self, arg)
    pass

class StateTransitionPort(Base):
    def __init__(self, dict):
        self.__dict__.update(dict)
    pass
    def accept(self, visitor, arg):
        return visitor.visitStateTransitionPort(self, arg)
    pass


class Parser(Base):
    C_EXPECTING_BEGIN = "EXPECTING_BEGIN"
    C_EXPECTING_END   = "EXPECTED_END"

    def __init__(self, fileName):
        self._FileName = fileName
        try:
            import queue
        except:
            import Queue as queue
        self._inputQueue = queue.Queue()

    def parse(self):
        try:
            return self._parse_i()
        except IOError as ioex:
            import traceback
            traceback.print_exc()
            raise
        except Exception as ex:
            import traceback
            traceback.print_exc()
            print ("Line:", self._LineNumber, " #", ex)
            raise
        pass
        return None

    def _parse_i(self):
        self._reader = self._openStateProtoFile(self._FileName)
        list = []
        headerdict = self._parseHeader()
        header = Header(headerdict)
        count = self._parseCount()
        for index in range(count):
            what = self._readLine()
            if what == "STATE:":
                statedict = self._parseState()
                state = State(statedict)
                list.append(state)
            elif what == "TRANSITION:":
                transitiondict = self._parseTransition()
                transition = Transition(transitiondict)
                list.append(transition)
            elif what == "STATETRANSITIONPORT:":
                statetransitionportdict = self._parseStateTransitionPort()
                statetransitionport = StateTransitionPort(statetransitionportdict)
                list.append(statetransitionport)
            else:
                raise "Could not process: " + what
            self._readExpectedLine("")
        pass
        return header, list

    def _parseHeader(self):
        statemachine = self._readBlock("StateMachine")
        implementationversion = self._readBlock("ImplementationVersion")
        statemachineversion = self._readBlock("StateMachineVersion")
        basestatemachine = self._readBlock("BaseStateMachine")
        namespace = self._readBlock("NameSpace")
        usingnamespaces = self._readBlock("UsingNameSpaces")
        comment = self._readBlock("Comment")
        fields = self._readBlock("Fields")
        readonly = self._readBlock("ReadOnly")
        modelfilename = self._readBlock("ModelFileName")
        modelguid = self._readBlock("ModelGuid")
        hassubmachines = self._readBlock("HasSubMachines")
        assembly = self._readBlock("Assembly")
        return locals()

    def _parseCount(self):
        count = int(next(self._reader))
        return count

    def _parseState(self):
        guid = self._readLine()
        left = self._readLine()
        top = self._readLine()
        width = self._readLine()
        height = self._readLine()
        parent = self._readLine()
        note = self._readBlock("NOTE:")
        donotinstrument = self._readBlock ("DEBUG_DONOTINSTRUMENT", str(False));
        name = self._readBlock("NAME")
        isstartstate = self._readBlock("ISSTARTSTATE")
        assert isstartstate in ["True", "False"]
        isfinalstate = self._readBlock("ISFINALSTATE", str(False))
        assert isfinalstate in ["True", "False"]
        entry = self._readBlock("ENTRY")
        exit = self._readBlock("EXIT")
        do = self._readBlock("DO")
        isoverriding = self._readBlock("ISOVERRIDING")
        statecommands = self._readBlock("STATECOMMANDS", "")
        return locals()

    def _parseTransition(self):
        guid = self._readLine()
        left = self._readLine()
        top = self._readLine()
        width = self._readLine()
        height = self._readLine()
        fromstate = self._readLine()
        tostate = self._readLine()
        note = self._readBlock("NOTE:")
        donotinstrument = self._readBlock ("DEBUG_DONOTINSTRUMENT", str(False));
        name = self._readBlock("NAME")
        event = self._readBlock("EVENT")
        guard = self._readBlock("GUARD")
        action = self._readBlock("ACTION")
        transitiontype = self._readBlock("TRANSITIONTYPE")
        eventsource = self._readBlock("EVENTSOURCE")
        eventtype = self._readBlock("EVENTTYPE", "")
        evaluationorderpriority = self._readBlock("EVALUATIONORDERPRIORITY")
        other = self._readLine()
        isInternalTransition = other == "True"
        timeoutexpression = self._readBlock("TIMEOUTEXPRESSION")
        return locals()

    def _parseStateTransitionPort(self):
        guid = self._readLine()
        left = self._readLine()
        top = self._readLine()
        width = self._readLine()
        height = self._readLine()
        note = self._readBlock("NOTE:")
        donotinstrument = self._readBlock ("DEBUG_DONOTINSTRUMENT", str(False));
        name = self._readBlock("NAME")
        return locals()

    def _openStateProtoFile(self, fileName):
        hFile = open(fileName, "r")
        self._LineNumber = 0
        while True:
            line = hFile.readline()
            if line == "":
                break
            self._LineNumber += 1
            line = line.strip()
            yield line
        yield None

    def _assert(self, condition, msg):
        if not condition:
            raise "Expecting: " + str(msg) + " at line: " + str(self._LineNumber)

    def _readExpectedLine(self, mustMatch):
        line = self._readLine()
        self._assert(line == mustMatch, "Line %s does not match expected %s" % (line, mustMatch,))
        return line

    def _readLine(self):
        if not self._inputQueue.empty():
            return self._inputQueue.get()
        pass
        return next(self._reader)

    def _pushBack(self, line):
        self._inputQueue.put(line)

    def _readBlock(self, expectedName, defaultValue = None):
        mode = self.C_EXPECTING_BEGIN
        blockContent = None
        while True:
            line = self._readLine()
            if mode == self.C_EXPECTING_BEGIN:
                if line != "BEGIN " + expectedName and defaultValue != None:
                    self._pushBack(line)
                    return defaultValue
                pass

                begin, name = line.split()

                mode = self.C_EXPECTING_END
                self._assert(begin == "BEGIN", "BEGIN")
                self._assert(expectedName == name, "Start of Block name %s different to expected block name %s" % (name, expectedName,))
            elif mode == self.C_EXPECTING_END:
                if line.startswith("END "):
                    end, name = line.split()
                    self._assert(end == "END", "END")
                    self._assert(expectedName == name, "End of Block name %s different to expected block name %s" % (name, expectedName,))
                    return blockContent
                else:
                    if blockContent == None:
                        blockContent = line
                    else:
                        blockContent += line
                    pass
                pass
            pass

        return None

class IsTypeVisitor:
    def __init__(self, expectedTypeString):
        self._ExpectedTypeString = expectedTypeString

    def visitState(self, item, arg):
        return self._ExpectedTypeString == "STATE"

    def visitTransition(self, item, arg):
        return self._ExpectedTypeString == "TRANSITION"

    def visitStateTransitionPort(self, item, arg):
        return self._ExpectedTypeString == "STATETRANSITIONPORT"

class ListOps(Base):
    def __init__(self, list):
        self._list = list

    def collect(self, func, arg):
        result = [func(item, arg) for item in self._list]
        return ListOps(results)

    def accept(self, func, arg):
        results = [item for item in self._list if func(item, arg)]
        return ListOps(results)

    def reject(self, func, arg):
        results = [item for item in self._list if not func(item, arg)]
        return ListOps(results)

    def reduce(self, startValue, func, arg):
        result = startValue
        for item in self._list:
            result = func(result, item, arg)
        return result

    def do(self, func, arg):
        for item in self._list:
            func(item, arg)
        pass

    def _toStream(self, stream):
        stream.write(self.__class__.__name__ + " {\n")
        stream.writeList(self._list)
        stream.write(" } //" + self.__class__.__name__  + "\n")

class ParsedModel(Base):
    def __init__(self, sm1FileName):
        self._NOPARENTSTATE = State({"name": "TOPSTATE", "guid": "NOPARENT", "entry": None, "exit": None, "isstartstate": None, "left":0, "top":0, "width":2, "height": 2})

        parser = Parser(sm1FileName)
        self._header, self._list = parser.parse()
        self._collectByGuid ()

        self._states = self.accept(lambda item, visitor: item.accept(visitor, None), IsTypeVisitor("STATE"))
        #print self._states
        self._transitions = self.accept(lambda item, visitor: item.accept(visitor, None), IsTypeVisitor("TRANSITION"))
        self._statetransitionports = self.accept(lambda item, visitor: item.accept(visitor, None), IsTypeVisitor("STATETRANSITIONPORT"))

        self._buildStateTree()
        self._updateDefaultStartState()
        self._updateStartingChildStates()
        self._updateBuildGroupedTransitions()

    def NoParentState(self):
        return self._NOPARENTSTATE

    def _collectByGuid(self):
        self._byguid = {}
        self._byguid["NOPARENT"] = self._NOPARENTSTATE
        for item in self._list:
            self._byguid[item.guid] = item
        pass
    def header(self):
        return self._header

    def ByGuid(self, guid):
        return self._byguid[guid]

    def collect(self, func, arg):
        return ListOps(self._list).collect(func, arg)

    def accept(self, func, arg):
        return ListOps(self._list).accept(func, arg)

    def reject(self, func, arg):
        return ListOps(self._list).reject(func, arg)

    def reduce(self, startValue, func, arg):
        return ListOps(self._list).reduce(startValue, func, arg)

    def do(self, func, arg):
        return ListOps(self._list).do(func, arg)

    def stateList(self):
        return self._states

    def transitionList(self):
        return self._transitions

    def statetransitionportList(self):
        return self._statetransitionports

    def stateTreeRoot(self):
        return self._stateTree

    def _buildStateTree(self):
        from . import StateTreeModel
        self._stateTree = StateTreeModel.buildStateTree(self)
        pass

    def _updateDefaultStartState(self):
        def updateState(stateNode, arg):
            assert stateNode.state().isstartstate in ["True", "False"]
            if stateNode.state().isstartstate == "True":
                arg.defaultStartState = stateNode.state()
            pass

        self._stateTree.do(updateState, self)
        pass

    def _updateStartingChildStates(self):
        def updateState(stateNode, arg):
            stateNode.state().childStartStateName = None
            stateNode.do(updateState, stateNode.state())
            assert stateNode.state().isstartstate in ["True", "False"]
            if stateNode.state().isstartstate == "True":
                arg.childStartStateName = stateNode.state().name
            pass

        self._stateTree.do(updateState, self._stateTree.state())
        pass

    def _updateBuildGroupedTransitions(self):
        def processState(state, arg):
            def getQualifiedEvent(transition):
                sv = "%s:%s" % (transition.eventsource, transition.event)
                return sv
            def processTransitions(transition, perSignal):
                key = getQualifiedEvent(transition)
                if not key in perSignal:
                    perSignal[key] = []
                perSignal[key].append(transition)
                pass

            list = self.transitionList().accept(lambda item, arg: item.fromstate == state.guid, None)
            perSignalGroupedInArray = {}
            list.do(processTransitions, perSignalGroupedInArray)

            def nonGuardedAheadOfGuarded_key(transition):
                if transition.guard is None:
                    return ""
                assert isinstance(transition.guard, str), transition.guard
                return transition.guard

            groupedTransitionList = []
            for key, value in perSignalGroupedInArray.items():
                value.sort(key=nonGuardedAheadOfGuarded_key,
                           reverse=True)
                transitionGroup = ListOps(value)
                transitionGroup.firstTransition = value[0]
                groupedTransitionList.append(transitionGroup)
                pass
            state.groupedTransitionList = ListOps(groupedTransitionList)
            pass

        self.stateList().do(processState, None)
        pass




class GenericPrintVisitor(Base):
    def __init__(self, parsedModel):
        self._parsedModel = parsedModel

    def isPrintToConsoleOn(self):
        return True

    def visitState(self, item, arg):
        if self.isPrintToConsoleOn():
            print ("S:", item.name,
                   self._parsedModel.ByGuid(item.parent).name,
                   item.entry, item.exit)
        pass
    def visitTransition(self, item, arg):
        if self.isPrintToConsoleOn():
            print ("T:", self._parsedModel.ByGuid(item.fromstate).name,
                   " -> ", item.event, " [ ", item.guard, "] / ", item.action,
                   " -> ", self._parsedModel.ByGuid(item.tostate).name)
        pass
    def visitStateTransitionPort(self, item, arg):
        if self.isPrintToConsoleOn():
            print ("P:", item.name)
        pass
