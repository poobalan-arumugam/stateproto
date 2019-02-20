#!/bin/bash

export PYTHONPATH=$(pwd):${PYTHONPATH}

cd reader

python3 ../reader/testStateProtoFileParser.py |& less -RS
