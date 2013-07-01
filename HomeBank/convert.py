#!/usr/bin/python

# converts some csv format into homebank readable csv

# "BANKOMAT 12345 KARTE66 1.01.UM 20:00 ";"01.01.2013";"-40,00";"EUR"
infile="28263611600_20090201_20090301.csv"

#        Column list:
#        date ; mode ; info ; payee ; description ; amount ; category
#
#        Values:
#        date     => format should be DD-MM-YY
#        mode     => from 0=none to 5=personal transfer
#        info     => a string
#        payee    => a payee name
#        description  => a string
#        amount   => a number with a '.' as decimal separator, ex: -24.12 or 36.75
#        category => a category name

import datetime
import sys

# use first argument as infile
#TODO use optparse
if len(sys.argv) >= 2:
    infile = sys.argv[1]
    outfile = infile.split('.')[0] + '_conv.' + infile.split('.')[1]
else:
    sys.exit(1)


with open(infile, 'r') as input_fh:
    with open(outfile, 'w') as output_fh:
        for line in input_fh.readlines()[1:]:
            try:
                (memo, date, amount, currency) = (line
                    .replace("\"", "")
                    .replace(",", ".")
                    .split(";")
                )
            except:
                continue
            (day, month, year) = map(int, date.split("."))
            date = datetime.date(year, month, day).strftime("%d-%m-%Y")
            mode = "0"
            info = ""
            payee = ""
            memo = (memo
                .decode('iso-8859-15')
                .encode('utf-8', 'strict')
                .strip())
            amount = str(float(amount))
            category = ""
            tags = ""
            output = [date, mode, info, payee, memo, amount, category, tags]
            output_fh.write(";".join(output) + '\n')
